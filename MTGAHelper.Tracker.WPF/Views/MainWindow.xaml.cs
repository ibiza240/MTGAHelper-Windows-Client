using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using MTGAHelper.Entity;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Lib.IO.Reader;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Business.Monitoring;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;
using MTGAHelper.Tracker.WPF.Views.UserControls;
using MTGAHelper.Web.Models.Response.Account;
using MTGAHelper.Web.UI.Model.Request;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MTGAHelper.Tracker.WPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindowVM vm;

        //OptionsWindow optionsWindow;

        public ConfigModelApp configApp;
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
        InGameTracker inGameTracker;
        ExternalProviderTokenManager tokenManager;
        PasswordHasher passwordHasher;

        public System.Windows.Forms.NotifyIcon trayIcon;

        public MainWindow(
            //OptionsWindow optionsWindow,
            IOptionsMonitor<ConfigModelApp> configApp,
            ICollection<Card> allCards,
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
            //CacheSingleton<ICollection<Card>> allCards,
            InGameTracker inMatchTracker,
            ExternalProviderTokenManager tokenManager,
            PasswordHasher passwordHasher
            )
        {
            this.configApp = configApp.CurrentValue;
            //optionsWindow.Init(this.configApp);
            //optionsWindow.Owner = Window.GetWindow(this);
            //this.optionsWindow = optionsWindow;

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
            this.inGameTracker = inMatchTracker;
            this.tokenManager = tokenManager;
            this.passwordHasher = passwordHasher;

            this.resourcesLocator.LocateLogFilePath(this.configApp);
            this.resourcesLocator.LocateGameClientFilePath(this.configApp);

            fileMonitor.SetFilePath(this.configApp.LogFilePath);
            //viewModel.ValidateUserId(this.configApp.UserId);
            viewModel.Opacity = this.configApp.Opacity;
            vm = viewModel;
            DataContext = vm;

            InitializeComponent();

            ucWelcome.Init(tokenManager);

            trayIcon = new System.Windows.Forms.NotifyIcon { Text = "MTGAHelper Tracker" };
            trayIcon.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/Assets/Images/wcC.ico")).Stream);
            trayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(TrayIcon_MouseClick);
            trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]
            {
                new System.Windows.Forms.MenuItem("Quit", new EventHandler(TrayIcon_Quit))
            });

            statusBarTop.Init(this, vm/*, draftHelper, logProcessor, this.configApp.UserId,*/);
            ucReady.Init(this.configApp.GameFilePath);
            ucDraftHelper.Init(allCards, vm.DraftingVM);
            //ucPlaying.Init(vm);

            ucDraftHelper.SetPopupRatingsSource(this.configApp.ShowLimitedRatings, this.configApp.ShowLimitedRatingsSource);

            this.processMonitor.Start(new System.Threading.CancellationToken());
            this.fileMonitor.Start(new System.Threading.CancellationToken());

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += (object sender, EventArgs e) =>
            {
                vm.SetCardsDraftFromBuffered();
                vm.SetCardsInMatchTrackingFromBuffered();
            };
            timer.Start();

            DispatcherTimer timerTokenRefresh = new DispatcherTimer();
            timerTokenRefresh.Interval = TimeSpan.FromMinutes(9);
            timerTokenRefresh.Tick += (object sender, EventArgs e) =>
            {
                RefreshAccessToken();
            };
            timerTokenRefresh.Start();
        }

        private void TrayIcon_Quit(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        void TrayIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ShowWindowFromTray();
                });
            }
        }

        public void ShowWindowFromTray()
        {
            trayIcon.Visible = false;
            this.Visibility = Visibility.Visible;
            //this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;
            //this.Show();
            //this.Focus();
            this.Activate();
        }

        //private void Window_StateChanged(object sender, EventArgs e)
        //{
        //    if (configApp.MinimizeToSystemTray == false)
        //        return;

        //    if (this.WindowState == WindowState.Minimized)
        //    {
        //        this.ShowInTaskbar = false;
        //        trayIcon.Visible = true;
        //        Visibility = Visibility.Hidden;
        //    }
        //    else if (trayIcon.Visible)
        //    {
        //        this.ShowInTaskbar = true;
        //        trayIcon.Visible = false;
        //        Visibility = Visibility.Visible;
        //        this.WindowState = WindowState.Normal;
        //        this.Activate();
        //    }
        //}
        //private void Window_StateChanged(object sender, EventArgs e)
        //{
        //    if (configApp.MinimizeToSystemTray && this.WindowState == WindowState.Minimized)
        //    {
        //        this.ShowInTaskbar = false;
        //        trayIcon.Visible = true;
        //        this.Visibility = Visibility.Hidden;
        //    }
        //}

        internal void ValidateExternalToken(string provider, string accessToken)
        {
            if (provider == "Facebook")
            {
                // Save the long-lived token for reuse
                vm.FacebookAccessToken = accessToken;
            }

            var info = api.ValidateExternalToken(provider, accessToken);
            if (info == null)
            {
                MessageBox.Show("Cannot sign-in");
            }
            else
            {
                SetSignedIn(info);
            }
        }

        internal void ValidateLocalUser(string password, bool rememberEmail, bool rememberPassword)
        {
            vm.SigninPassword = password;

            if (string.IsNullOrWhiteSpace(vm.SigninEmail.Value) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Invalid email/password", "MTGAHelper");
                return;
            }

            var info = api.ValidateLocalUser(vm.SigninEmail.Value, password);
            if (info == null)
            {
                MessageBox.Show("Cannot sign-in", "MTGAHelper");
            }
            else if (info.IsAuthenticated)
            {
                var signinPassword = "";
                if (rememberPassword)
                {
                    var salt = api.GetAccountSalt(vm.SigninEmail.Value);
                    signinPassword = passwordHasher.Hash(password, salt);
                }

                configApp.SigninEmail = rememberEmail || rememberPassword ? vm.SigninEmail.Value : "";
                configApp.SigninPassword = signinPassword;

                SetSignedIn(info); // This saves the configApp

            }
            else
            {
                MessageBox.Show(info.Message, "MTGAHelper");
            }
        }

        internal void SetSignedIn(AccountResponse account)
        {
            configApp.SigninProvider = account.Provider;
            configApp.Save();

            vm.SetSignedIn(account);
            //api.Init(account, password);
            api.SetUserId(account.MtgaHelperUserId);

            //void ServerApiGetCollection()
            {
                if (vm.CanUpload == false || vm.Account.IsAuthenticated == false)
                    return;

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        vm.WrapNetworkStatus(NetworkStatusEnum.Downloading, () =>
                        {
                            var collection = api.GetCollection(vm.Account.MtgaHelperUserId);
                            vm.SetCollection(collection);
                        });

                        UploadLogFile();
                    }
                    catch (HttpRequestException ex)
                    {
                        vm.SetProblemServerUnavailable();
                    }
                });
            }
        }

        internal void ShowDialogOptions()
        {
            try
            {
                var optionsWindow = new OptionsWindow().Init(this.configApp);
                optionsWindow.Owner = this;
                optionsWindow.ShowDialog();

                // The code will continue here only when the options window gets closed
                var newConfig = JsonConvert.DeserializeObject<ConfigModelApp>(JsonConvert.SerializeObject(configApp));
                newConfig.LogFilePath = optionsWindow.txtLogFilePath.Text.Trim();
                newConfig.GameFilePath = optionsWindow.txtGameFilePath.Text.Trim();
                newConfig.RunOnStartup = optionsWindow.chkRunOnStartup.IsChecked.Value;
                newConfig.MinimizeToSystemTray = optionsWindow.chkMinimizeToSystemTray.IsChecked.Value;
                newConfig.AutoShowHideForMatch = optionsWindow.chkAutoShowHideForMatch.IsChecked.Value;
                newConfig.ForceCardPopup = optionsWindow.chkForceCardPopup.IsChecked.Value;
                newConfig.ForceCardPopupSide = optionsWindow.lstForceCardPopupSide?.SelectedValue?.ToString() ?? configApp.ForceCardPopupSide;
                newConfig.ShowLimitedRatingsSource = optionsWindow.lstShowLimitedRatingsSource?.SelectedValue?.ToString() ?? configApp.ShowLimitedRatingsSource;

                if (JsonConvert.SerializeObject(configApp) != JsonConvert.SerializeObject(newConfig))
                {
                    var oldLimitedRatings = configApp.ShowLimitedRatings;
                    var oldLimitedRatingsSource = configApp.ShowLimitedRatingsSource;
                    newConfig.Save();
                    configApp = newConfig;

                    if (vm.MainWindowContext == MainWindowContextEnum.Drafting &&
                        (configApp.ShowLimitedRatings != oldLimitedRatings || configApp.ShowLimitedRatingsSource != oldLimitedRatingsSource))
                    {
                        if (configApp.ShowLimitedRatingsSource != oldLimitedRatingsSource)
                            SetCardsDraft(vm.DraftingVM.CurrentDraftPickProgress);

                        ucDraftHelper.SetPopupRatingsSource(configApp.ShowLimitedRatings, configApp.ShowLimitedRatingsSource);
                    }

                    resourcesLocator.LocateLogFilePath(configApp);
                    resourcesLocator.LocateGameClientFilePath(configApp);
                    fileMonitor.SetFilePath(newConfig.LogFilePath);
                    //vm.ValidateUserId(newConfig.UserId);
                    //ServerApiGetCollection();

                    startupManager.ManageRunOnStartup(newConfig.RunOnStartup);
                    ucReady.Init(configApp.GameFilePath);
                }

                UpdateCardPopupPosition();
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

            //RefreshAccessToken();

            fileMonitor.ResetStringBuilder();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var uploadHash = logSplitter.GetLastUploadHash(logToSend);
                    if (api.IsSameLastUploadHash(vm.Account.MtgaHelperUserId, uploadHash))
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
                            vm.WrapNetworkStatus(NetworkStatusEnum.ProcessingLogFile, () =>
                            {
                                (result, errorId) = reader.LoadFileContent(vm.Account.MtgaHelperUserId, ms);
                            });

                            if (result.CollectionByDate.Any(i => i.DateTime == default(DateTime)))
                                api.LogErrorRemoteFile(vm.Account.MtgaHelperUserId, logToSend, $"_NODATE_outputlog_{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip");

                            if (errorId.HasValue)
                                api.LogErrorRemoteFile(vm.Account.MtgaHelperUserId, logToSend, $"_parsererror_{errorId}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip");
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Problem processing log piece ({logSize})", logToSend.Length);
                            api.LogErrorRemoteFile(vm.Account.MtgaHelperUserId, logToSend, $"_unknownError{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip");
                        }
                    }

                    var collection = result.GetLastCollection();
                    vm.WrapNetworkStatus(NetworkStatusEnum.Uploading, () =>
                    {
                        var collection = api.UploadOutputLogResult(vm.Account.MtgaHelperUserId, result);
                        vm.SetCollection(collection);
                    });
                }
                catch (HttpRequestException ex)
                {
                    callbackOnError?.Invoke();
                    Log.Error(ex, "Error:");
                    vm.SetProblemServerUnavailable();
                }
            });
        }

        private void RefreshAccessToken()
        {
            if (vm.Account.IsAuthenticated == false)
                return;

            if (vm.Account.Provider == null)
            {
                api.ValidateLocalUser(vm.SigninEmail.Value, vm.SigninPassword);
            }
            else
            {
                string token = null;
                switch (vm.Account.Provider)
                {
                    case "Google":
                        token = tokenManager.GoogleRefresh();
                        break;
                    case "Facebook":
                        //token = tokenManager.FacebookRefresh();
                        // Always re-use the long-lived token
                        token = vm.FacebookAccessToken;
                        break;
                }
                api.ValidateExternalToken(vm.Account.Provider, token);
            }
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

                // Only log when the problem is not that it just is already uploading
                if (activeStatus.Length != 1 || activeStatus[0] != NetworkStatusEnum.Uploading)
                    Log.Warning("FileSizeChangedNewText() Could not upload data. Status:{status} - Problems:{problems}", string.Join(',', activeStatus), vm.ProblemsList);
            });
        }

        internal void SetAlwaysOnTop(bool alwaysOnTop)
        {
            statusBarTop.SetAlwaysOnTop(alwaysOnTop);
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

            //if (messages.Count == 20) System.Diagnostics.Debugger.Break();

            Action GoHome = () =>
            {
                SetMainWindowContext(MainWindowContextEnum.Home);
            };

            foreach (var msg in messages)
            {
                if (msg is IResultCardPool msgCardPool)
                {
                    var isDrafting = true;
                    if (msg is GetEventPlayerCourseV2Result playerCourse)
                    {
                        if (playerCourse.Raw.CurrentEventState == "PreMatch")
                        {
                            // Clear the drafting window
                            SetCardsDraft(new DraftPickProgress(new int[0]));
                        }

                        isDrafting = playerCourse.Raw.InternalEventName.Contains("Draft") || playerCourse.Raw.InternalEventName.Contains("Sealed");
                    }

                    if (isDrafting)
                    {
                        // Refresh the drafting window to show whole card pool
                        SetCardsDraft(new DraftPickProgress(msgCardPool.CardPool));
                        SetMainWindowContext(MainWindowContextEnum.Drafting);
                    }
                }
                else if (msg is IResultDraftPick msgDraftPack && msgDraftPack.Raw.draftPack != null)
                {
                    // Refresh the drafting window to show the new picks
                    var draftInfo = Mapper.Map<DraftPickProgress>(msgDraftPack.Raw);
                    SetCardsDraft(draftInfo);
                }

                if (msg is LogInfoRequestResult logInfo)
                {
                    if (logInfo.Raw.@params.messageName == "DuelScene.EndOfMatchReport" || logInfo.Raw.@params.humanContext.Contains("Client changed scenes to Home"))
                    {
                        // Trigger to upload the stored log content
                        UploadLogFragment();
                    }
                }

                if (vm.MainWindowContext != MainWindowContextEnum.Playing && msg is GetPlayerCardsResult)
                {
                    // Safety trigger to upload the stored log content
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
                                ((string)prms.payloadObject.context).Contains("Draft") == false &&
                                ((string)prms.payloadObject.context).Contains("Opening sealed boosters") == false &&
                                ((string)prms.payloadObject.context) != "deck builder")
                            {
                                GoHome();
                            }
                            break;
                        case "DuelScene.EndOfMatchReport":
                            GoHome();
                            break;
                    }
                }
                else if (msg is StateChangedResult stateChanged)
                {
                    if (msg.Part.Contains("MatchCompleted"))
                        GoHome();

                }
                else if (msg is MatchCreatedResult matchCreated)
                {
                    SetMainWindowContext(MainWindowContextEnum.Playing);
                }
                //else if (msg is EventClaimPrizeResult claimPrize)
                //{
                //    // Clear the drafting window
                //    SetCardsDraft(new int[0]);
                //}
                else if (msg is ConnectRespResult connectResp)
                {
                    inGameTracker.InitSideboard(
                        connectResp.Raw.systemSeatIds.First(),
                        connectResp.Raw.connectResp.deckMessage.sideboardCards);
                }

                if (vm.MainWindowContext == MainWindowContextEnum.Playing)
                {
                    inGameTracker.ProcessMessage(msg);
                    vm.SetInMatchStateBuffered(inGameTracker.State);
                }
            }
        }

        void SetMainWindowContext(MainWindowContextEnum context)
        {
            if (configApp.AutoShowHideForMatch)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (context == MainWindowContextEnum.Home)
                    {
                        statusBarTop.MinimizeWindow();
                    }
                    else
                    {
                        if (configApp.MinimizeToSystemTray)
                            ShowWindowFromTray();
                        else
                            WindowState = WindowState.Normal;
                    }
                });
            }

            vm.SetMainWindowContext(context);
        }

        void SetCardsDraft(DraftPickProgress draftInfo)
        {
            var cardPool = draftInfo?.DraftPack;

            if (cardPool != null)
            {
                vm.WrapNetworkStatusInNewTask(NetworkStatusEnum.Downloading, () =>
                {
                    var cardsInfoFromServer = draftHelper.GetDraftPicksForCards(vm.Account.MtgaHelperUserId, cardPool, configApp.ShowLimitedRatingsSource);

                    //if (msg is GetEventPlayerCourseV2Result course)
                    //{
                    //    course.Raw.CourseDeck.
                    //}

                    vm.SetCardsDraftBuffered(draftInfo, cardsInfoFromServer);
                });
            }
        }
    }
}
