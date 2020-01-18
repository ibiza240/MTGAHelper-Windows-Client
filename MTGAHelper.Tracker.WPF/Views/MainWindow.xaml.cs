using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AutoMapper;
using Microsoft.Extensions.Options;
using MTGAHelper.Entity;
using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Business.Monitoring;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;
using MTGAHelper.Web.Models.Response.Account;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;

namespace MTGAHelper.Tracker.WPF.Views
{
    public partial class MainWindow : Window
    {
        public readonly MainWindowVM vm;

        //OptionsWindow optionsWindow;

        public ConfigModelApp configApp;
        readonly ProcessMonitor processMonitor;
        readonly LogFileZipper zipper;
        readonly ServerApiCaller api;
        readonly StartupShortcutManager startupManager;
        readonly LogSplitter logSplitter;
        readonly MtgaResourcesLocator resourcesLocator;
        readonly FileMonitor fileMonitor;

        readonly DraftHelper draftHelper;
        //LogProcessor logProcessor;
        readonly ReaderMtgaOutputLog reader;
        readonly InGameTracker inGameTracker;
        readonly ExternalProviderTokenManager tokenManager;
        readonly PasswordHasher passwordHasher;

        public NotifyIconManager notifyIconManager;
        private readonly CacheSingleton<Dictionary<string, DraftRatings>> draftRatings;

        //public System.Windows.Forms.NotifyIcon trayIcon;

        ICollection<CardCompareInfo> raredraftingInfo;

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
            PasswordHasher passwordHasher,
            NotifyIconManager notifyIconManager,
            CacheSingleton<Dictionary<string, DraftRatings>> draftRatings
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
            this.notifyIconManager = notifyIconManager;
            this.draftRatings = draftRatings;
            this.resourcesLocator.LocateLogFilePath(this.configApp);
            this.resourcesLocator.LocateGameClientFilePath(this.configApp);

            fileMonitor.SetFilePath(this.configApp.LogFilePath);
            //viewModel.ValidateUserId(this.configApp.UserId);
            viewModel.Opacity = this.configApp.Opacity;
            vm = viewModel;
            DataContext = vm;

            InitializeComponent();

            ucWelcome.Init(tokenManager);
            ucPlaying.Init(vm, this.configApp.WindowSettingsOpponentCards);

            //trayIcon = new System.Windows.Forms.NotifyIcon { Text = "MTGAHelper Tracker" };
            //trayIcon.Icon = new System.Drawing.Icon(Application.GetResourceStream(new Uri("pack://application:,,,/Assets/Images/wcC.ico")).Stream);
            //trayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(TrayIcon_MouseClick);
            //trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]
            //{
            //    new System.Windows.Forms.MenuItem("Quit", new EventHandler(TrayIcon_Quit))
            //});

            statusBarTop.Init(this, vm/*, draftHelper, logProcessor, this.configApp.UserId,*/);
            ucReady.Init(this.configApp.GameFilePath);
            ucDraftHelper.Init(allCards, vm.DraftingVM);
            //ucPlaying.Init(vm);

            ucDraftHelper.SetPopupRatingsSource(this.configApp.ShowLimitedRatings, this.configApp.ShowLimitedRatingsSource);

            this.processMonitor.Start(new System.Threading.CancellationToken());
            this.fileMonitor.Start(new System.Threading.CancellationToken());

            var timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(200)};
            timer.Tick += (object sender, EventArgs e) =>
            {
                vm.SetCardsDraftFromBuffered();
                vm.SetCardsInMatchTrackingFromBuffered();
            };
            timer.Start();

            var timerTokenRefresh = new DispatcherTimer {Interval = TimeSpan.FromMinutes(9)};
            timerTokenRefresh.Tick += (object sender, EventArgs e) =>
            {
                RefreshAccessToken();
            };
            timerTokenRefresh.Start();
        }

        internal void UpdateWindowOpponentCardsPosition(int top, int left, int width)
        {
            configApp.WindowSettingsOpponentCards = new WindowSettings
            {
                Position = new Config.Point(left, top),
                Size = new Config.Point(width, 0),
            };
        }

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
                            RefreshRaredraftingInfo();
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

        private void RefreshRaredraftingInfo()
        {
            vm.WrapNetworkStatus(NetworkStatusEnum.Downloading, () =>
            {
                raredraftingInfo = api.GetRaredraftingInfo(vm.Account.MtgaHelperUserId).Data;
            });
        }

        internal void ShowDialogOptions()
        {
            try
            {
                var ratingSources = draftRatings.Get().Keys.ToArray();
                var optionsWindow = new OptionsWindow().Init(this.configApp, ratingSources);
                optionsWindow.Owner = this;
                optionsWindow.ShowDialog();

                // The code will continue here only when the options window gets closed
                var newConfig = JsonConvert.DeserializeObject<ConfigModelApp>(JsonConvert.SerializeObject(configApp));
                newConfig.LogFilePath = optionsWindow.txtLogFilePath.Text.Trim();
                newConfig.GameFilePath = optionsWindow.txtGameFilePath.Text.Trim();
                newConfig.RunOnStartup = optionsWindow.chkRunOnStartup.IsChecked.Value;
                newConfig.ShowOpponentCards = optionsWindow.chkShowOpponentCards.IsChecked.Value;
                newConfig.MinimizeToSystemTray = optionsWindow.chkMinimizeToSystemTray.IsChecked.Value;
                newConfig.AutoShowHideForMatch = optionsWindow.chkAutoShowHideForMatch.IsChecked.Value;
                newConfig.ForceCardPopup = optionsWindow.chkForceCardPopup.IsChecked.Value;
                newConfig.OrderLibraryCardsBy = optionsWindow.lstOrderLibraryCardsBy.SelectedValue.ToString();
                newConfig.ForceCardPopupSide = optionsWindow.lstForceCardPopupSide?.SelectedValue?.ToString() ?? configApp.ForceCardPopupSide;
                newConfig.ShowLimitedRatingsSource = optionsWindow.lstShowLimitedRatingsSource?.SelectedValue?.ToString() ?? configApp.ShowLimitedRatingsSource;

                ShowInTaskbar = !newConfig.MinimizeToSystemTray;
                if (configApp.MinimizeToSystemTray != newConfig.MinimizeToSystemTray)
                {
                    if (newConfig.MinimizeToSystemTray)
                        notifyIconManager.AddNotifyIcon(this);
                    else
                        notifyIconManager.RemoveNotifyIcon();
                }

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

                    vm.OrderLibraryCardsBy = configApp.OrderLibraryCardsBy == "Converted Mana Cost" ? CardsListOrder.Cmc : CardsListOrder.DrawChance;
                }

                UpdateCardPopupPosition();
            }
            catch (Exception ex)
            {
                Log.Write(LogEventLevel.Error, ex, "Unexpected error:");
            }
        }

        void UploadInfoToServer(bool isLive, string logToSend, Action callbackOnError = null)
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
                    //if (api.IsSameLastUploadHash(vm.Account.MtgaHelperUserId, uploadHash))
                    //{
                    //    vm.WrapNetworkStatus(NetworkStatusEnum.UpToDate, () => Task.Delay(5000).Wait());
                    //    return;
                    //}

                    OutputLogResult result = null;
                    Guid? errorId = null;
                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(logToSend ?? "")))
                    {
                        try
                        {
                            vm.WrapNetworkStatus(NetworkStatusEnum.ProcessingLogFile, () =>
                            {
                                (result, errorId) = reader.LoadFileContent(isLive, vm.Account.MtgaHelperUserId, ms);
                            });

                            if (result.CollectionByDate.Any(i => i.DateTime == default(DateTime)))
                                api.LogErrorRemoteFile(vm.Account.MtgaHelperUserId, logToSend, $"_NODATE_outputlog_{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip");

                            if (errorId.HasValue)
                            {
                                api.LogErrorRemoteFile(vm.Account.MtgaHelperUserId, logToSend, $"_parsererror_{errorId}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Problem processing log piece ({logSize})", logToSend.Length);
                            api.LogErrorRemoteFile(vm.Account.MtgaHelperUserId, logToSend, $"_unknownError{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip");
                        }
                    }

                    vm.WrapNetworkStatus(NetworkStatusEnum.Uploading, () =>
                    {
                        var collection = api.UploadOutputLogResult(vm.Account.MtgaHelperUserId, result);
                        vm.SetCollection(collection);
                        RefreshRaredraftingInfo();
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
            try
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
            catch (Exception ex)
            {
                Log.Error("Remote server was not accessible for token refresh");
            }
        }

        public void UploadLogFragment(Action callbackOnError = null)
        {
            var logToSend = fileMonitor.LogContentToSend.ToString();
            UploadInfoToServer(true, logToSend, () =>
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
            ucPlaying.SetAlwaysOnTop(alwaysOnTop);
        }

        public void UploadLogFile(Action callbackOnError = null)
        {
            if (File.Exists(configApp.LogFilePath) && new FileInfo(configApp.LogFilePath).Length > 0)
            {
                var logContent = zipper.ReadLogFile(configApp.LogFilePath);
                UploadInfoToServer(false, logContent, callbackOnError);
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
            vm.SetProblem(ProblemsFlags.LogFileNotFound, false);
            vm.SizeOfLogToSend = fileMonitor.LogContentToSend.Length;

            ICollection<IMtgaOutputLogPartResult> messages = new IMtgaOutputLogPartResult[0];
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(newText ?? "")))
            {
                messages = reader.ProcessIntoMessages(true, "local", ms);
            }

            //if (messages.Count == 20) System.Diagnostics.Debugger.Break();

            Action GoHome = () =>
            {
                SetMainWindowContext(MainWindowContextEnum.Home);
            };

            var mustUpload = false;

            foreach (var msg in messages)
            {
                if (msg is IResultCardPool msgCardPool)
                {
                    var isDrafting = true;
                    if (msg is GetEventPlayerCourseV2Result playerCourse)
                    {
                        if (playerCourse.Raw.payload.CurrentEventState == "PreMatch")
                        {
                            // Clear the drafting window
                            SetCardsDraft(new DraftPickProgress(new int[0]));
                        }

                        isDrafting = playerCourse.Raw.payload.InternalEventName.Contains("Draft") || playerCourse.Raw.payload.InternalEventName.Contains("Sealed");
                    }

                    if (isDrafting)
                    {
                        // Refresh the drafting window to show whole card pool
                        SetCardsDraft(new DraftPickProgress(msgCardPool.CardPool));
                        SetMainWindowContext(MainWindowContextEnum.Drafting);
                    }
                }
                else if (msg is IResultDraftPick msgDraftPack && msgDraftPack.Raw.payload.draftPack != null)
                {
                    // Refresh the drafting window to show the new picks
                    var draftInfo = Mapper.Map<DraftPickProgress>(msgDraftPack.Raw.payload);
                    SetCardsDraft(draftInfo);
                }
                else if (msg is LogInfoRequestResult logInfoContainer)
                {
                    var logInfo = JsonConvert.DeserializeObject<LogInfoRequestInnerRaw>(logInfoContainer.Raw.request);
                    if (logInfo.@params.messageName == "DuelScene.EndOfMatchReport" || logInfo.@params.humanContext.Contains("Client changed scenes to Home"))
                    {
                        //// Trigger to upload the stored log content
                        //UploadLogFragment();
                        mustUpload = true;
                    }

                    // Change MainWindowContext
                    var prms = logInfo.@params;
                    switch (prms.messageName)
                    {
                        case "Client.SceneChange":
                            var context = prms.payloadObject.context?.ToString();   // SceneChange_PayloadObject
                            if (prms.humanContext.Contains("Client changed scene") &&
                                context.Contains("Draft") == false &&
                                context.Contains("Opening sealed boosters") == false &&
                                context != "deck builder")
                            {
                                GoHome();
                            }
                            break;
                        case "DuelScene.EndOfMatchReport":
                            GoHome();
                            inGameTracker.Reset();
                            vm.SetInMatchStateBuffered(inGameTracker.State);
                            break;
                    }
                }
                else if (msg is StateChangedResult stateChanged)
                {
                    if (msg.Part.Contains("\"new\":8}"))
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
                else if (msg is GetPlayerInventoryResult || msg is GetPlayerCardsResult ||
                    msg is PostMatchUpdateResult || msg is RankUpdatedResult ||
                    msg is GetCombinedRankInfoResult)
                {
                    mustUpload = true;
                }
                else if (msg is InventoryUpdatedResult)
                {
                    mustUpload = true;
                }

                if (vm.MainWindowContext == MainWindowContextEnum.Playing)
                {
                    inGameTracker.ProcessMessage(msg);
                    vm.SetInMatchStateBuffered(inGameTracker.State);
                }
            }

            if (mustUpload)
            {
                UploadLogFragment();
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
                            notifyIconManager.ShowWindowFromTray();
                        else
                            WindowState = WindowState.Normal;

                        var process = Process.GetProcessesByName("MTGA").FirstOrDefault();
                        BringToFront(process);
                    }
                });
            }

            if (configApp.ShowOpponentCards)
                ucPlaying.ShowHideOpponentCards(context == MainWindowContextEnum.Playing);

            vm.SetMainWindowContext(context);
        }

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        private void BringToFront(Process pTemp)
        {
            try
            {
                SetForegroundWindow(pTemp.MainWindowHandle);
            }
            catch (Exception ex)
            {
                // Doesn't matter
            }
        }

        void SetCardsDraft(DraftPickProgress draftInfo)
        {
            var cardPool = draftInfo?.DraftPack;

            if (cardPool != null)
            {
                var draftingInfo = draftHelper.GetDraftPicksForCards(
                    vm.Account.MtgaHelperUserId,
                    cardPool,
                    configApp.ShowLimitedRatingsSource,
                    vm.Collection.Cards.Where(i => i.IdArena != 0).ToDictionary(i => i.IdArena, i => i.Amount),
                    raredraftingInfo);

                vm.SetCardsDraftBuffered(draftInfo, draftingInfo);
            }
        }
    }
}
