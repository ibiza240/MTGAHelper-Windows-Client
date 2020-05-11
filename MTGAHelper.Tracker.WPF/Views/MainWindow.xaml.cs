using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Business.Monitoring;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Tracker.WPF.Tools;
using MTGAHelper.Tracker.WPF.ViewModels;
using MTGAHelper.Web.Models.Response.Account;
using MTGAHelper.Web.UI.Model.Response.User;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Point = System.Windows.Point;

namespace MTGAHelper.Tracker.WPF.Views
{
    public partial class MainWindow
    {
        /// <summary>
        /// The main window view model
        /// </summary>
        public MainWindowVM MainWindowVM { get; }

        /// <summary>
        /// The configuration settings
        /// </summary>
        public ConfigModel ConfigModel { get; private set; }


        private readonly LogFileZipper Zipper;
        private readonly ServerApiCaller Api;
        private readonly StartupShortcutManager StartupManager;
        private readonly LogSplitter LogSplitter;
        private readonly MtgaResourcesLocator ResourcesLocator;
        private readonly FileMonitor FileMonitor;
        private readonly DraftCardsPicker DraftHelper;
        private readonly ReaderMtgaOutputLog Reader;
        private readonly InGameTracker2 InGameTracker;
        private readonly ExternalProviderTokenManager TokenManager;
        private readonly PasswordHasher PasswordHasher;
        private readonly CacheSingleton<Dictionary<string, DraftRatings>> DraftRatings;
        private readonly DraftHelperRunner DraftHelperRunner;
        private ICollection<CardCompareInfo> RareDraftingInfo;

        public MainWindow(
            ConfigModel configApp,
            ICollection<Card> allCards,
            MainWindowVM viewModel,
            ProcessMonitor processMonitor,
            LogFileZipper zipper,
            ServerApiCaller api,
            StartupShortcutManager startupManager,
            LogSplitter logSplitter,
            MtgaResourcesLocator resourcesLocator,
            FileMonitor fileMonitor,
            DraftCardsPicker draftHelper,
            ReaderMtgaOutputLog readerMtgaOutputLog,
            InGameTracker2 inMatchTracker,
            ExternalProviderTokenManager tokenManager,
            PasswordHasher passwordHasher,
            CacheSingleton<Dictionary<string, DraftRatings>> draftRatings,
            DraftHelperRunner draftHelperRunner)
        {
            // Set the config model reference
            ConfigModel = configApp;


            Reader = readerMtgaOutputLog;
            processMonitor.OnProcessMonitorStatusChanged += OnProcessMonitorStatusChanged;
            Zipper = zipper;
            Api = api;
            StartupManager = startupManager;
            LogSplitter = logSplitter;
            ResourcesLocator = resourcesLocator;
            FileMonitor = fileMonitor;
            fileMonitor.OnFileSizeChangedNewText += OnFileSizeChangedNewText;
            DraftHelper = draftHelper;
            //this.logProcessor = logProcessor;
            InGameTracker = inMatchTracker;
            TokenManager = tokenManager;
            PasswordHasher = passwordHasher;
            DraftRatings = draftRatings;
            DraftHelperRunner = draftHelperRunner;
            ResourcesLocator.LocateLogFilePath(ConfigModel);
            ResourcesLocator.LocateGameClientFilePath(ConfigModel);

            fileMonitor.SetFilePath(ConfigModel.LogFilePath);

            // Set the view model
            MainWindowVM = viewModel;

            // Set the data context to the view model
            DataContext = MainWindowVM;

            InitializeComponent();

            WelcomeControl.Init(tokenManager);
            PlayingControl.Init(MainWindowVM);
            StatusBarTop.Init(this, MainWindowVM);
            ReadyControl.Init(ConfigModel.GameFilePath);
            DraftingControl.Init(allCards, MainWindowVM.DraftingVM);

            DraftingControl.SetPopupRatingsSource(ConfigModel.ShowLimitedRatings, ConfigModel.ShowLimitedRatingsSource);

            processMonitor.Start(new System.Threading.CancellationToken());

            FileMonitor.Start(new System.Threading.CancellationToken());

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            timer.Tick += (sender, e) =>
            {
                MainWindowVM.SetCardsDraftFromBuffered();
                MainWindowVM.SetCardsInMatchTrackingFromBuffered();
            };
            timer.Start();

            var timerTokenRefresh = new DispatcherTimer { Interval = TimeSpan.FromMinutes(9) };
            timerTokenRefresh.Tick += (sender, e) =>
            {
                RefreshAccessToken();
            };
            timerTokenRefresh.Start();
        }

        internal void ValidateExternalToken(string provider, string accessToken)
        {
            if (provider == "Facebook")
            {
                // Save the long-lived token for reuse
                MainWindowVM.FacebookAccessToken = accessToken;
            }

            AccountResponse info = Api.ValidateExternalToken(provider, accessToken);

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
            MainWindowVM.SigninPassword = password;

            if (string.IsNullOrWhiteSpace(MainWindowVM.SigninEmail.Value) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Invalid email/password", "MTGAHelper");
                return;
            }

            AccountResponse info = Api.ValidateLocalUser(MainWindowVM.SigninEmail.Value, password);

            if (info == null)
            {
                MessageBox.Show("Cannot sign-in", "MTGAHelper");
            }
            else if (info.IsAuthenticated)
            {
                var signinPassword = "";
                if (rememberPassword)
                {
                    string salt = Api.GetAccountSalt(MainWindowVM.SigninEmail.Value);
                    signinPassword = PasswordHasher.Hash(password, salt);
                }

                ConfigModel.SigninEmail = rememberEmail || rememberPassword ? MainWindowVM.SigninEmail.Value : "";
                ConfigModel.SigninPassword = signinPassword;

                SetSignedIn(info); // This saves the configApp

            }
            else
            {
                MessageBox.Show(info.Message, "MTGAHelper");
            }
        }

        private void SetSignedIn(AccountResponse account)
        {
            ConfigModel.SigninProvider = account.Provider;

            ConfigModel.Save();

            MainWindowVM.SetSignedIn(account);

            Api.SetUserId(account.MtgaHelperUserId);

            if (MainWindowVM.CanUpload == false || MainWindowVM.Account.IsAuthenticated == false)
                return;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    MainWindowVM.WrapNetworkStatus(NetworkStatusEnum.Downloading, () =>
                    {
                        CollectionResponse collection = Api.GetCollection(MainWindowVM.Account.MtgaHelperUserId);
                        MainWindowVM.SetCollection(collection);
                        RefreshRareDraftingInfo();
                    });

                    UploadLogFile();
                }
                catch (HttpRequestException)
                {
                    MainWindowVM.SetProblemServerUnavailable();
                }
            });

        }

        private void RefreshRareDraftingInfo()
        {
            MainWindowVM.WrapNetworkStatus(NetworkStatusEnum.Downloading, () =>
            {
                RareDraftingInfo = Api.GetRaredraftingInfo(MainWindowVM.Account.MtgaHelperUserId).Data;
            });
        }

        internal void ShowDialogOptions()
        {
            try
            {
                var ratingSources = DraftRatings.Get().Keys.ToArray();
                OptionsWindow optionsWindow = new OptionsWindow().Init(ConfigModel, ratingSources);
                optionsWindow.Owner = this;
                optionsWindow.ShowDialog();

                // The code will continue here only when the options window gets closed
                var newConfig = JsonConvert.DeserializeObject<ConfigModel>(JsonConvert.SerializeObject(ConfigModel));
                newConfig.LogFilePath = optionsWindow.LogFilePathTextBox.Text.Trim();
                newConfig.GameFilePath = optionsWindow.GameFilePathTextBox.Text.Trim();
                newConfig.RunOnStartup = optionsWindow.RunOnStartupCheckbox.IsChecked ?? false;
                newConfig.ShowOpponentCardsAuto = optionsWindow.ShowOpponentCardsCheckbox.IsChecked ?? false;
                newConfig.ShowOpponentCardsExternal = optionsWindow.ShowOpponentCardsExternalCheckBox.IsChecked ?? true;
                newConfig.MinimizeToSystemTray = optionsWindow.MinimizeToTrayCheckBox.IsChecked ?? false;
                newConfig.AutoShowHideForMatch = optionsWindow.AutoShowHideForMatchCheckBox.IsChecked ?? false;
                newConfig.ForceCardPopup = optionsWindow.ForceCardPopupCheckbox.IsChecked ?? false;
                newConfig.OrderLibraryCardsBy = optionsWindow.OrderLibraryComboBox.SelectedValue.ToString();
                newConfig.ForceCardPopupSide = optionsWindow.ForceCardPopupSideComboBox?.SelectedValue?.ToString() ?? ConfigModel.ForceCardPopupSide;
                newConfig.ShowLimitedRatingsSource = optionsWindow.ShowLimitedRatingsSourceComboBox?.SelectedValue?.ToString() ?? ConfigModel.ShowLimitedRatingsSource;

                if (JsonConvert.SerializeObject(ConfigModel) != JsonConvert.SerializeObject(newConfig))
                {
                    bool oldLimitedRatings = ConfigModel.ShowLimitedRatings;
                    string oldLimitedRatingsSource = ConfigModel.ShowLimitedRatingsSource;
                    newConfig.Save();
                    Utilities.CopyProperties(newConfig, ConfigModel);

                    if (MainWindowVM.MainWindowContext == WindowContext.Drafting &&
                        (ConfigModel.ShowLimitedRatings != oldLimitedRatings || ConfigModel.ShowLimitedRatingsSource != oldLimitedRatingsSource))
                    {
                        if (ConfigModel.ShowLimitedRatingsSource != oldLimitedRatingsSource)
                            SetCardsDraft(MainWindowVM.DraftingVM.CurrentDraftPickProgress);

                        DraftingControl.SetPopupRatingsSource(ConfigModel.ShowLimitedRatings, ConfigModel.ShowLimitedRatingsSource);
                    }

                    ResourcesLocator.LocateLogFilePath(ConfigModel);
                    ResourcesLocator.LocateGameClientFilePath(ConfigModel);
                    FileMonitor.SetFilePath(newConfig.LogFilePath);
                    //vm.ValidateUserId(newConfig.UserId);
                    //ServerApiGetCollection();

                    StartupManager.ManageRunOnStartup(newConfig.RunOnStartup);
                    ReadyControl.Init(ConfigModel.GameFilePath);

                    MainWindowVM.OrderLibraryCardsBy = ConfigModel.OrderLibraryCardsBy == "Converted Mana Cost" ? CardsListOrder.ManaCost : CardsListOrder.DrawChance;
                }

                UpdateCardPopupPosition();
            }
            catch (Exception ex)
            {
                Log.Write(LogEventLevel.Error, ex, "Unexpected error:");
            }
        }

        private void UploadInfoToServer(bool isLive, string logToSend, Action callbackOnError = null)
        {
            if (MainWindowVM.CanUpload == false)
            {
                callbackOnError?.Invoke();
                return;
            }

            //RefreshAccessToken();

            FileMonitor.ResetStringBuilder();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    LogSplitter.GetLastUploadHash(logToSend);
                    //if (api.IsSameLastUploadHash(vm.Account.MtgaHelperUserId, uploadHash))
                    //{
                    //    vm.WrapNetworkStatus(NetworkStatusEnum.UpToDate, () => Task.Delay(5000).Wait());
                    //    return;
                    //}

                    OutputLogResult result = null;
                    Guid? errorId = null;
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(logToSend ?? "")))
                    {
                        try
                        {
                            MainWindowVM.WrapNetworkStatus(NetworkStatusEnum.ProcessingLogFile, () =>
                            {
                                (result, errorId) = Reader.LoadFileContent(MainWindowVM.Account.MtgaHelperUserId, ms).Result;
                            });

                            if (result.CollectionByDate.Any(i => i.DateTime == default))
                                Api.LogErrorRemoteFile(MainWindowVM.Account.MtgaHelperUserId, logToSend, $"_NODATE_outputlog_{DateTime.Now:yyyyMMddHHmmss}.zip");

                            if (errorId.HasValue)
                            {
                                Api.LogErrorRemoteFile(MainWindowVM.Account.MtgaHelperUserId, logToSend, $"_parsererror_{errorId}_{DateTime.Now:yyyyMMddHHmmss}.zip");
                            }
                        }
                        catch (Exception ex)
                        {
                            if (logToSend != null)
                            {
                                Log.Error(ex, "Problem processing log piece ({logSize})", logToSend.Length);

                                Api.LogErrorRemoteFile(MainWindowVM.Account.MtgaHelperUserId, logToSend,
                                    $"_unknownError{DateTime.Now:yyyyMMddHHmmss}.zip");
                            }
                        }
                    }

                    MainWindowVM.WrapNetworkStatus(NetworkStatusEnum.Uploading, () =>
                    {

#if !DEBUG && !DEBUGWITHSERVER
                        CollectionResponse collection = Api.UploadOutputLogResult(MainWindowVM.Account.MtgaHelperUserId, result);
                        MainWindowVM.SetCollection(collection);
#else
                        Console.WriteLine("Running in DEBUG - no server upload!");
#endif

                        RefreshRareDraftingInfo();
                    });
                }
                catch (HttpRequestException ex)
                {
                    callbackOnError?.Invoke();
                    Log.Error(ex, "Error:");
                    MainWindowVM.SetProblemServerUnavailable();
                }
            });
        }

        private void RefreshAccessToken()
        {
            try
            {
                if (MainWindowVM.Account.IsAuthenticated == false)
                    return;

                if (MainWindowVM.Account.Provider == null)
                {
                    Api.ValidateLocalUser(MainWindowVM.SigninEmail.Value, MainWindowVM.SigninPassword);
                }
                else
                {
                    string token = null;
                    switch (MainWindowVM.Account.Provider)
                    {
                        case "Google":
                            token = TokenManager.GoogleRefresh();
                            break;
                        case "Facebook":
                            //token = tokenManager.FacebookRefresh();
                            // Always re-use the long-lived token
                            token = MainWindowVM.FacebookAccessToken;
                            break;
                    }

                    if (string.IsNullOrWhiteSpace(token) == false)
                        Api.ValidateExternalToken(MainWindowVM.Account.Provider, token);
                }
            }
            catch (Exception)
            {
                Log.Error("Remote server was not accessible for token refresh");
            }
        }

        private void UploadLogFragment(Action callbackOnError = null)
        {
            var logToSend = FileMonitor.LogContentToSend.ToString();
            UploadInfoToServer(true, logToSend, () =>
            {
                NetworkStatusEnum flags = MainWindowVM.GetFlagsNetworkStatus();
                var activeStatus = Enum.GetValues(typeof(NetworkStatusEnum)).Cast<NetworkStatusEnum>()
                    .Where(i => i != NetworkStatusEnum.Ready)
                    .Where(i => flags.HasFlag(i))
                    .ToArray();

                // Only log when the problem is not that it just is already uploading
                if (activeStatus.Length != 1 || activeStatus[0] != NetworkStatusEnum.Uploading)
                    Log.Warning("FileSizeChangedNewText() Could not upload data. Status:{status} - Problems:{problems}", string.Join(",", activeStatus), MainWindowVM.ProblemsList);
            });
        }

        public void UploadLogFile(Action callbackOnError = null)
        {
            if (File.Exists(ConfigModel.LogFilePath) && new FileInfo(ConfigModel.LogFilePath).Length > 0)
            {
                string logContent = LogFileZipper.ReadLogFile(ConfigModel.LogFilePath);
                UploadInfoToServer(false, logContent, callbackOnError);
                Reader.ResetPreparer();
            }
            else
                callbackOnError?.Invoke();
        }

        private void OnProcessMonitorStatusChanged(object sender, bool isRunning)
        {
            MainWindowVM.SetIsGameRunning(isRunning);

            if (isRunning)
                FileMonitor.ResetStringBuilder();
            else
                UploadLogFile();
        }

        private void OnFileSizeChangedNewText(object sender, string newText)
        {
            try
            {
                HandleNewText(newText);
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Unexpected error:");
                throw;
            }
        }

        private void HandleNewText(string newText)
        {
            MainWindowVM.UnSetProblem(ProblemsFlags.LogFileNotFound);
            MainWindowVM.SizeOfLogToSend = FileMonitor.LogContentToSend.Length;

            ICollection<IMtgaOutputLogPartResult> messages;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(newText ?? "")))
            {
                messages = Reader.ProcessIntoMessagesAsync("local", ms).Result;
            }

            void GoHome()
            {
                MainWindowVM.SetMainWindowContext(WindowContext.Home);
            }

            var mustUpload = false;

            foreach (IMtgaOutputLogPartResult msg in messages)
            {
                switch (msg)
                {
                    case UnknownResult _:
                    case IgnoredResult _:
                        continue;
                    case DetailedLoggingResult detLogRes:
                        MainWindowVM.SetProblem(ProblemsFlags.DetailedLogsDisabled, detLogRes.IsDetailedLoggingDisabled);
                        break;
                    case IResultCardPool msgCardPool:
                        {
                            var isDrafting = true;
                            if (msg is GetEventPlayerCourseV2Result playerCourse)
                            {
                                GetEventPlayerCourseV2Raw payload = playerCourse.Raw.payload;
                                if (payload.CurrentEventState == "PreMatch")
                                {
                                    // Clear the drafting window
                                    SetCardsDraft(new DraftPickProgress(new int[0]));
                                }

                                isDrafting = payload.InternalEventName.Contains("Draft") || payload.InternalEventName.Contains("Sealed");
                            }

                            if (isDrafting)
                            {
                                // Refresh the drafting window to show whole card pool
                                SetCardsDraft(new DraftPickProgress(msgCardPool.CardPool));
                                MainWindowVM.SetMainWindowContext(WindowContext.Drafting);
                            }

                            break;
                        }
                    case IResultDraftPick msgDraftPack when msgDraftPack.Raw.payload.draftPack != null:
                        {
                            // Refresh the drafting window to show the new picks
                            var draftInfo = Mapper.Map<DraftPickProgress>(msgDraftPack.Raw.payload);
                            SetCardsDraft(draftInfo);
                            break;
                        }
                    case LogInfoRequestResult logInfoContainer:
                        {
                            var logInfo = JsonConvert.DeserializeObject<LogInfoRequestInnerRaw>(logInfoContainer.Raw.request);
                            if (logInfo.@params.messageName == "DuelScene.EndOfMatchReport" || logInfo.@params.humanContext.Contains("Client changed scenes to Home"))
                            {
                                //// Trigger to upload the stored log content
                                //UploadLogFragment();
                                mustUpload = true;
                            }

                            // Change MainWindowContext
                            Params prms = logInfo.@params;
                            switch (prms.messageName)
                            {
                                case "Client.SceneChange":
                                    dynamic context = prms.payloadObject.context?.ToString();   // SceneChange_PayloadObject
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
                                    InGameTracker.Reset();
                                    MainWindowVM.SetInMatchStateBuffered(InGameTracker.State);
                                    break;
                            }

                            break;
                        }
                    case StateChangedResult stateChanged:
                        {
                            if (msg.Part.Contains("\"new\":8}"))
                                GoHome();
                            break;
                        }
                    //else if (msg is EventClaimPrizeResult claimPrize)
                    //{
                    //    // Clear the drafting window
                    //    SetCardsDraft(new int[0]);
                    //}
                    case MatchCreatedResult matchCreated:
                        MainWindowVM.SetMainWindowContext(WindowContext.Playing);
                        break;
                    case GetPlayerInventoryResult _:
                    case GetPlayerCardsResult _:
                    case PostMatchUpdateResult _:
                    case RankUpdatedResult _:
                    case GetCombinedRankInfoResult _:
                    case InventoryUpdatedResult _:
                        mustUpload = true;
                        break;
                }

                if (MainWindowVM.MainWindowContext == WindowContext.Playing)
                {
                    InGameTracker.ProcessMessage(msg);
                    MainWindowVM.SetInMatchStateBuffered(InGameTracker.State);
                }
            }

            if (mustUpload)
            {
                UploadLogFragment();
            }
        }

        private void SetCardsDraft(DraftPickProgress draftInfo)
        {
            var cardPool = draftInfo?.DraftPack;
            if (cardPool == null)
                return;

            var draftingInfo = DraftHelper.GetDraftPicksForCards(
                MainWindowVM.Account.MtgaHelperUserId,
                cardPool,
                ConfigModel.ShowLimitedRatingsSource,
                MainWindowVM.Collection.Cards.Where(i => i.IdArena != 0).ToDictionary(i => i.IdArena, i => i.Amount),
                RareDraftingInfo);

            MainWindowVM.SetCardsDraftBuffered(draftInfo, draftingInfo);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCardPopupPosition();

            if (string.IsNullOrWhiteSpace(ConfigModel.SigninProvider) == false)
            {
                string token = ConfigModel.SigninProvider switch
                {
                    "Google" => await TokenManager.GoogleSignin(),
                    "Facebook" => TokenManager.FacebookSignin(this),
                    _ => null
                };

                ValidateExternalToken(ConfigModel.SigninProvider, token);
            }
            else if (string.IsNullOrWhiteSpace(ConfigModel.SigninEmail) == false)
            {
                if (string.IsNullOrWhiteSpace(ConfigModel.SigninPassword) == false)
                {
                    AccountResponse info = Api.AutoSigninLocalUser(ConfigModel.SigninEmail, ConfigModel.SigninPassword);
                    if (info == null)
                    {
                        MessageBox.Show("Cannot auto-signin the local account", "MTGAHelper");
                    }
                    else if (info.IsAuthenticated)
                    {
                        SetSignedIn(info);
                    }
                }
                else
                {
                    MainWindowVM.SigninEmail.Value = ConfigModel.SigninEmail;
                    WelcomeControl.SetRememberEmail();
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Dispose the notify icon so it disappear after closing the application
            NotifyIcon?.Dispose();

            // Close the application
            Application.Current.Shutdown();
        }

        private void StatusBarTop_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (WindowState == WindowState.Maximized)
            {
                Point mousePos = PointToScreen(Mouse.GetPosition(this));
                Top = 0;
                Left = mousePos.X - 20;
                WindowState = WindowState.Normal;
            }

            // Begin dragging the window
            DragMove();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            UpdateCardPopupPosition();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCardPopupPosition();
        }

        private void UpdateCardPopupPosition()
        {
            if (double.IsNaN(Top) || double.IsNaN(Left) ||
                double.IsNaN(Width) || double.IsNaN(Height))
                return;

            var top = (int)Top;
            var left = (int)Left;
            var width = (int)Width;
            ForceCardPopupSideEnum side = GetCardPopupSide();

            DraftingControl.SetCardPopupPosition(side, top, left, width);
            PlayingControl.SetCardPopupPosition(side, top, left, width);
        }

        private ForceCardPopupSideEnum GetCardPopupSide()
        {
            var side = ForceCardPopupSideEnum.None;
            if (ConfigModel.ForceCardPopup)
            {
                side = ConfigModel.ForceCardPopupSide.Contains("left") ? ForceCardPopupSideEnum.Left : ForceCardPopupSideEnum.Right;
            }

            return side;
        }

        public void RunDraftHelper()
        {
            var folderCommunication = ConfigModel.DraftHelperFolderCommunication.Replace("%AppData%",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

            var validation =
                DraftHelperRunner.Validate(folderCommunication, Path.GetFileName(ConfigModel.GameFilePath));
            if (validation != DraftHelperRunnerValidationResultEnum.Success)
            {
                MessageBox.Show(validation switch
                {
                    DraftHelperRunnerValidationResultEnum.InputConfigMissing =>
                    "You must first configure the draft helper",
                    DraftHelperRunnerValidationResultEnum.MtgaProgramNotRunning => "The MTGA game is not running",
                    _ => "Unknown error",
                });
                return;
            }

            MainWindowVM.MinimizeWindow();

            DraftHelperRunner.Run(ConfigModel.ShowLimitedRatingsSource, ConfigModel.DraftHelperFolder);

            var outputFile = Path.Combine(folderCommunication, "output.json");
            if (File.Exists(outputFile))
            {
                string outputContent = File.ReadAllText(outputFile);
                var cardIds = JsonConvert.DeserializeObject<ICollection<DraftHelperOutputModel>>(outputContent)
                    .Select(i => i.CardId).ToArray();

                SetCardsDraft(new DraftPickProgress(cardIds));
                MainWindowVM.SetMainWindowContext(WindowContext.Drafting);
            }
            else
            {
                MessageBox.Show("Output not found");
            }

            MainWindowVM.RestoreWindow();
        }
    }
}
