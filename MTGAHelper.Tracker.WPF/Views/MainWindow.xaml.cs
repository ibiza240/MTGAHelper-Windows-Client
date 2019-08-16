using Microsoft.Extensions.Options;
using Microsoft.Win32;
using MTGAHelper.Entity;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Lib.IO.Reader;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Business.Monitoring;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;
using MTGAHelper.Web.UI.Model.Request;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MTGAHelper.Tracker.WPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindowVM vm;

        ConfigModelApp configApp;
        ProcessMonitor processMonitor;
        LogFileZipper zipper;
        ServerApiCaller api;
        StartupShortcutManager startupManager;
        LogSplitter logSplitter;
        MtgaResourcesLocator resourcesLocator;
        FileMonitor fileMonitor;
        DraftHelper draftHelper;
        //LogProcessor logProcessor;
        ReaderMtgaOutputLog reader;


        public CardPopupDrafting windowCardPopupDrafting = new CardPopupDrafting();

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
            DraftHelper draftHelper,
            //LogProcessor logProcessor,
            ReaderMtgaOutputLog readerMtgaOutputLog,
            CacheSingleton<ICollection<Card>> allCards
            )
        {
            this.configApp = configApp.CurrentValue;
            this.reader = readerMtgaOutputLog;
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
            //this.logProcessor = logProcessor;

            fileMonitor.SetFilePath(this.configApp.LogFilePath);
            viewModel.ValidateUserId(this.configApp.UserId);
            DataContext = viewModel;
            vm = viewModel;

            InitializeComponent();

            statusBarTop.Init(this, vm, /*draftHelper, logProcessor, this.configApp.UserId,*/ allCards.Get());
            ucReady.Init(this.configApp.GameFilePath);
            ucDraftHelper.Init(vm.DraftingVM);

            this.processMonitor.Start(new System.Threading.CancellationToken());
            this.fileMonitor.Start(new System.Threading.CancellationToken());

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += (object sender, EventArgs e) =>
            {
                vm.SetCardsDraftFromBuffered();
            };
            timer.Start();
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

        void UploadInfoToServer(string logToSend, Action callbackOnError = null)
        {
            if (vm.CanUpload == false)
            {
                callbackOnError?.Invoke();
                return;
            }

            fileMonitor.ResetStringBuilder();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var uploadHash = logSplitter.GetLastUploadHash(logToSend);
                    if (api.IsSameLastUploadHash(configApp.UserId, uploadHash))
                    {
                        vm.WrapNetworkStatus(NetworkStatusEnum.UpToDate, () => Task.Delay(5000).Wait());
                        return;
                    }

                    OutputLogResult result = null;
                    Guid? errorId = null;
                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(logToSend ?? "")))
                    {
                        try
                        {
                            (result, errorId) = reader.LoadFileContent(configApp.UserId, ms);

                            if (result.CollectionByDate.Any(i => i.DateTime == default(DateTime)))
                                api.LogErrorRemoteFile(configApp.UserId, logToSend, $"_NODATE_outputlog_{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip");

                            if (errorId.HasValue)
                                api.LogErrorRemoteFile(configApp.UserId, logToSend, $"_parsererror_{errorId}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip");
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Problem processing log piece ({logSize})", logToSend.Length);
                            api.LogErrorRemoteFile(configApp.UserId, logToSend, $"_unknownError{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip");
                        }
                    }

                    var collection = result.GetLastCollection();
                    vm.WrapNetworkStatus(NetworkStatusEnum.Uploading, () =>
                    {
                        var collection = api.UploadOutputLogResult(configApp.UserId, result);
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

        public void UploadLogFragment(Action callbackOnError = null)
        {
            var logToSend = fileMonitor.LogContentToSend.ToString();
            UploadInfoToServer(logToSend, () =>
            {
                var flags = vm.GetFlagsNetworkStatus();
                var activeStatus = Enum.GetValues(typeof(NetworkStatusEnum)).Cast<NetworkStatusEnum>()
                    .Where(i => i != NetworkStatusEnum.Ready)
                    .Where(i => flags.HasFlag(i))
                    .ToArray();

                Log.Warning("FileSizeChangedNewText() Could not upload data. Status:{status} - Problems:{problems}", string.Join(',', activeStatus), vm.ProblemsList);
            });
        }

        public void UploadLogFile(Action callbackOnError = null)
        {
            if (File.Exists(configApp.LogFilePath) && new FileInfo(configApp.LogFilePath).Length > 0)
            {
                var logContent = zipper.ReadLogFile(configApp.LogFilePath);
                UploadInfoToServer(logContent, callbackOnError);
                reader.ResetPreparer();
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

        public void OnFileSizeChangedNewText(object sender, string newText)
        {
            vm.SizeOfLogToSend = fileMonitor.LogContentToSend.Length;

            ICollection<IMtgaOutputLogPartResult> messages = new IMtgaOutputLogPartResult[0];
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(newText ?? "")))
            {
                messages = reader.ProcessIntoMessages("local", ms);
            }

            foreach (var msg in messages)
            {
                if (msg is IResultCardPool msgCardPool)
                    // Refresh the drafting window to show whole card pool
                    SetCardsDraft(msgCardPool.CardPool);
                else if (msg is IResultDraftPick msgDraftPack && msgDraftPack.DraftPack != null)
                    // Refresh the drafting window to show the new picks
                    SetCardsDraft(msgDraftPack.DraftPack.Select(i => Convert.ToInt32(i)).ToArray());

                if ((msg is LogInfoRequestResult logInfo && logInfo.Raw.@params.messageName == "DuelScene.EndOfMatchReport") ||
                    (vm.MainWindowContext != MainWindowContextEnum.Playing && msg is GetPlayerCardsResult))
                {
                    // Trigger to upload the stored log content
                    UploadLogFragment();
                }

                // Change MainWindowContext
                if (msg is LogInfoRequestResult logInfo2)
                {
                    var prms = logInfo2.Raw.@params;
                    switch (prms.messageName)
                    {
                        case "Client.SceneChange":
                            if (prms.humanContext.Contains("Client changed scene") &&
                                ((string)prms.payloadObject.context).Contains("Draft") == false && ((string)prms.payloadObject.context) != "deck builder")
                            {
                                vm.SetMainWindowContext(MainWindowContextEnum.Home);
                            }
                            break;
                        case "DuelScene.EndOfMatchReport":
                            vm.SetMainWindowContext(MainWindowContextEnum.Home);
                            break;
                    }
                }
                else if (msg is MatchCreatedResult matchCreated)
                {
                    vm.SetMainWindowContext(MainWindowContextEnum.Playing);
                }
            }
        }

        void SetCardsDraft(ICollection<int> cardPool)
        {
            if (cardPool != null)
            {
                vm.WrapNetworkStatusInNewTask(NetworkStatusEnum.Downloading, () =>
                {
                    var cardsInfo = draftHelper.GetDraftPicksForCards(configApp.UserId, cardPool);

                    //if (msg is GetEventPlayerCourseV2Result course)
                    //{
                    //    course.Raw.CourseDeck.
                    //}

                    vm.SetCardsDraftBuffered(cardsInfo);
                });
            }
        }

    }
    #endregion
}
