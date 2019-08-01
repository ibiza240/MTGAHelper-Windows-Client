using Microsoft.Extensions.Options;
using Microsoft.Win32;
using MTGAHelper.Entity;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Business.Monitoring;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MTGAHelper.Tracker.WPF.Views
{
    public partial class MainWindow : Window
    {
        ConfigModelApp configApp;
        ProcessMonitor processMonitor;
        LogFileZipper zipper;
        ServerApiCaller api;
        StartupShortcutManager startupManager;
        LogSplitter logSplitter;
        MainWindowVM vm;
        MtgaResourcesLocator resourcesLocator;
        FileMonitor fileMonitor;
        DraftHelper draftHelper;

        public MainWindow(
            IOptionsMonitor<ConfigModelApp> configApp,
            MainWindowVM viewModel,
            ProcessMonitor processMonitor,
            LogFileZipper zipper,
            ServerApiCaller api,
            StartupShortcutManager startupManager,
            LogSplitter logSplitter,
            MtgaResourcesLocator resourcesLocator,
            FileMonitor fileMonitor,
            DraftHelper draftHelper)
        {
            this.configApp = configApp.CurrentValue;
            this.processMonitor = processMonitor;
            processMonitor.OnProcessMonitorStatusChanged += OnProcessMonitorStatusChanged;
            this.zipper = zipper;
            this.api = api;
            this.startupManager = startupManager;
            this.logSplitter = logSplitter;
            this.resourcesLocator = resourcesLocator;
            this.fileMonitor = fileMonitor;
            fileMonitor.OnFileSizeChangedNewText += OnFileSizeChangedNewText;
            this.draftHelper = draftHelper;

            fileMonitor.SetFilePath(this.configApp.LogFilePath);
            viewModel.ValidateUserId(this.configApp.UserId);
            DataContext = viewModel;
            vm = viewModel;

            InitializeComponent();

            ucReady.Init(this.configApp.GameFilePath);

            this.processMonitor.Start(new System.Threading.CancellationToken());
            this.fileMonitor.Start(new System.Threading.CancellationToken());
        }

        internal void ShowDialogOptions()
        {
            try
            {
                var optionsWindow = new OptionsWindow().Init(configApp);
                optionsWindow.Owner = Window.GetWindow(this);
                optionsWindow.ShowDialog();

                // The code will continue here only when the options window gets closed
                var newConfig = new ConfigModelApp
                {
                    UserId = optionsWindow.txtUserId.Text,
                    LogFilePath = optionsWindow.txtLogFilePath.Text,
                    GameFilePath = optionsWindow.txtGameFilePath.Text,
                    RunOnStartup = optionsWindow.chkRunOnStartup.IsChecked ?? false,
                };

                if (JsonConvert.SerializeObject(configApp) != JsonConvert.SerializeObject(newConfig))
                {
                    newConfig.Save();
                    configApp = newConfig;

                    resourcesLocator.LocateLogFilePath(configApp);
                    resourcesLocator.LocateGameClientFilePath(configApp);
                    fileMonitor.SetFilePath(newConfig.LogFilePath);
                    vm.ValidateUserId(newConfig.UserId);
                    InitialServerApiCalls();

                    startupManager.ManageRunOnStartup(newConfig.RunOnStartup);
                    ucReady.Init(configApp.GameFilePath);
                }
            }
            catch (Exception ex)
            {
                Log.Write(LogEventLevel.Error, ex, "Unexpected error:");
            }
        }

        void UploadInfoToServer(byte[] zipFile, string uploadHash, Action callbackOnError = null)
        {
            if (vm.CanUpload == false)
            {
                callbackOnError?.Invoke();
                return;
            }

            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (api.IsSameLastUploadHash(configApp.UserId, uploadHash))
                    {
                        vm.WrapNetworkStatus(NetworkStatusEnum.UpToDate, () => Task.Delay(5000).Wait());
                        return;
                    }

                    if (api.IsLocalTrackerUpToDate() == false)
                        MustDownloadNewVersion();

                    vm.WrapNetworkStatus(NetworkStatusEnum.Uploading, () =>
                    {
                        var collection = api.UploadZippedLogFile(configApp.UserId, zipFile);
                        vm.SetCollection(collection);
                    });
                }
                catch (HttpRequestException ex)
                {
                    callbackOnError?.Invoke();
                    vm.SetProblemServerUnavailable();
                }
            });
        }

        public void UploadLogFile(Action callbackOnError = null)
        {
            if (File.Exists(configApp.LogFilePath) && new FileInfo(configApp.LogFilePath).Length > 0)
            {
                var logContent = zipper.ReadLogFile(configApp.LogFilePath);
                var zipFile = zipper.ZipText(logContent);
                var uploadHash = logSplitter.GetLastUploadHash(logContent);
                UploadInfoToServer(zipFile, uploadHash, callbackOnError);
            }
            else
                callbackOnError?.Invoke();
        }

        #region Event handlers
        void OnProcessMonitorStatusChanged(object sender, bool isRunning)
        {
            vm.SetIsGameRunning(isRunning);

            if (isRunning)
                fileMonitor.ResetStringBuilder();
            else
                UploadLogFile();
        }

        void OnFileSizeChangedNewText(object sender, string newText)
        {
            if (newText.Contains("<== Draft.MakePick"))
            {
                vm.WrapNetworkStatus(NetworkStatusEnum.Downloading, () =>
                {
                    var cardsInfo = draftHelper.ParseDraftMakePick(configApp.UserId, newText);
                    vm.SetCardsDraft(cardsInfo);
                });
            }

            vm.SetLogContentNewText(newText, fileMonitor.LogContentToSend.Length);

            if (vm.IsInMatch == false && newText.Contains("<== PlayerInventory.GetPlayerCardsV3"))
            {
                // Trigger to upload the stored log content
                var logToSend = fileMonitor.LogContentToSend.ToString();
                var zipped = zipper.ZipText(logToSend);
                var uploadHash = logSplitter.GetLastUploadHash(logToSend);

                UploadInfoToServer(zipped, uploadHash, () =>
                {
                    var flags = vm.GetFlagsNetworkStatus();
                    var activeStatus = Enum.GetValues(typeof(NetworkStatusEnum)).Cast<NetworkStatusEnum>()
                        .Where(i => i != NetworkStatusEnum.Ready)
                        .Where(i => flags.HasFlag(i))
                        .ToArray();

                    Log.Warning("FileSizeChangedNewText() Could not upload data. Status:{status} - Problems:{problems}", string.Join(',', activeStatus, vm.ProblemsList));
                });

                fileMonitor.ResetStringBuilder();
            }
        }
    }
    #endregion
}
