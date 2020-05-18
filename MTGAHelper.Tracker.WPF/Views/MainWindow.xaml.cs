using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
using MTGAHelper.Tracker.DraftHelper.Shared.Config;
using MTGAHelper.Tracker.DraftHelper.Shared.Exceptions;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Business.Monitoring;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.Tools;
using MTGAHelper.Tracker.WPF.ViewModels;
using MTGAHelper.Web.Models.Response.Account;
using MTGAHelper.Web.UI.Model.Response.User;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Point = System.Windows.Point;
using Visibility = System.Windows.Visibility;

namespace MTGAHelper.Tracker.WPF.Views
{
    public partial class MainWindow
    {
        /// <summary>
        /// Primary View Model
        /// </summary>
        public MainWindowVM ViewModel { get; }

        /// <summary>
        /// Singleton Configuration Settings
        /// </summary>
        public ConfigModel Config { get; }

        /// <summary>
        /// DraftRating for enumerating the sources
        /// </summary>
        private CacheSingleton<Dictionary<string, DraftRatings>> DraftRatings { get; }

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
        private readonly DraftHelperRunner DraftHelperRunner;
        private ICollection<CardCompareInfo> RareDraftingInfo;
        private readonly IEmailProvider EmailProvider;

        /// <summary>
        /// Complete constructor
        /// </summary>
        /// <param name="configModel"></param>
        /// <param name="allCards"></param>
        /// <param name="viewModel"></param>
        /// <param name="processMonitor"></param>
        /// <param name="zipper"></param>
        /// <param name="api"></param>
        /// <param name="startupManager"></param>
        /// <param name="logSplitter"></param>
        /// <param name="resourcesLocator"></param>
        /// <param name="fileMonitor"></param>
        /// <param name="draftHelper"></param>
        /// <param name="readerMtgaOutputLog"></param>
        /// <param name="inMatchTracker"></param>
        /// <param name="tokenManager"></param>
        /// <param name="passwordHasher"></param>
        /// <param name="draftRatings"></param>
        /// <param name="draftHelperRunner"></param>
        /// <param name="emailProvider"></param>
        public MainWindow(
            ConfigModel configModel,
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
            DraftHelperRunner draftHelperRunner,
            IEmailProvider emailProvider)
        {
            // Set the config singleton reference
            Config = configModel;

            // Set the view model
            ViewModel = viewModel;

            // Set the main window view model upload log action
            ViewModel.UploadLogAction = UploadLogAction;

            // Set the main window show options action
            ViewModel.ShowOptionsAction = ShowOptionsWindow;

            // Set the main window launch Arena action
            ViewModel.LaunchArenaAction = LaunchArenaAction;

            // Set the main window user validate action
            ViewModel.ValidateUserAction = ValidateUserAction;

            // Set the resource locator
            ResourcesLocator = resourcesLocator;

            // Set the problem action
            ResourcesLocator.SetProblem = ViewModel.SetProblem;

            // Locate the log file
            ResourcesLocator.LocateLogFilePath(Config);

            // Locate the game path
            ResourcesLocator.LocateGameClientFilePath(Config);

            // Set the reference to the draft helper
            DraftHelperRunner = draftHelperRunner;


            Reader = readerMtgaOutputLog;
            Zipper = zipper;
            Api = api;
            StartupManager = startupManager;
            LogSplitter = logSplitter;
            FileMonitor = fileMonitor;
            FileMonitor.OnFileSizeChangedNewText += OnFileSizeChangedNewText;
            DraftHelper = draftHelper;
            //this.logProcessor = logProcessor;
            InGameTracker = inMatchTracker;
            TokenManager = tokenManager;
            PasswordHasher = passwordHasher;
            DraftRatings = draftRatings;

            EmailProvider = emailProvider;

            FileMonitor.SetFilePath(Config.LogFilePath);

            // Set the data context to the view model
            DataContext = ViewModel;

            InitializeComponent();

            PlayingControl.Init(ViewModel);
            DraftingControl.Init(allCards, ViewModel.DraftingVM);
            DraftingControl.SetPopupRatingsSource(Config.ShowLimitedRatings, Config.LimitedRatingsSource);

            // Set the process monitor status changed action
            processMonitor.OnProcessMonitorStatusChanged = OnProcessMonitorStatusChanged;

            // Start the process monitor without awaiting the task completion
            _ = processMonitor.Start(new System.Threading.CancellationToken());

            // Start the file monitor without awaiting the task completion
            _ = FileMonitor.Start(new System.Threading.CancellationToken());

            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            timer.Tick += (sender, e) =>
            {
                ViewModel.DraftingVM.SetCardsDraftFromBuffered();
                ViewModel.InMatchState.SetInMatchStateFromBuffered();
            };
            timer.Start();

            var timerTokenRefresh = new DispatcherTimer { Interval = TimeSpan.FromMinutes(9) };
            timerTokenRefresh.Tick += (sender, e) =>
            {
                RefreshAccessToken();
            };
            timerTokenRefresh.Start();
        }

        /// <summary>
        /// Method for validating a user based on the given sign-in type
        /// </summary>
        /// <param name="obj">String version of requested sign-in authentication</param>
        private async void ValidateUserAction(object obj)
        {
            // If the provided argument is not a string, attempt local account authentication
            if (!(obj is string s))
                s = "";

            switch (s)
            {
                case "Google":
                {
                    string token = await TokenManager.GoogleSignin();

                    ValidateExternalToken("Google", token);

                    break;
                }
                case "Facebook":
                {
                    string token = TokenManager.FacebookSignin(this);

                    ValidateExternalToken("Facebook", token);

                    break;
                }
                default:
                {
                    ValidateLocalUser();

                    break;
                }
            }
        }

        /// <summary>
        /// Method for uploading the log file with generic callback
        /// </summary>
        private void UploadLogAction()
        {
            // Execute the upload with a custom callback
            UploadLogFile(() => Dispatcher.Invoke(() =>
            {
                // Join the problem strings
                string errors = ViewModel.ProblemsList.Count == 0
                    ? "."
                    : $":{Environment.NewLine}{string.Join(Environment.NewLine, ViewModel.ProblemsList)}";

                // Show a message box to the user
                MessageBox.Show($"Could not upload the log file{errors}", "MTGAHelper", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        /// <summary>
        /// Method for launching the Arena game client
        /// </summary>
        private void LaunchArenaAction()
        {
            try
            {
                // Confirm that the path is not null
                if (Config?.GameFilePath == null)
                    return;

                // Check if the process is already running
                if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Config.GameFilePath)).Length > 0)
                {
                    MessageBox.Show("Game is already running.", "MTGAHelper");
                    return;
                }

                // Modify the path to point to the launcher
                string launcherPath = Config.GameFilePath.Replace("MTGA.exe", "MTGALauncher/MTGALauncher.exe");

                // Get the process start info
                var ps = new ProcessStartInfo(launcherPath);

                // Start the process
                Process.Start(ps);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error launching the game: {ex.Message}", "MTGAHelper");
            }
        }

        /// <summary>
        /// Validate the user using an external token
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="accessToken"></param>
        private void ValidateExternalToken(string provider, string accessToken)
        {
            if (provider == "Facebook")
            {
                // Save the long-lived token for reuse
                ViewModel.FacebookAccessToken = accessToken;
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

        /// <summary>
        /// Validate the local user account
        /// </summary>
        private void ValidateLocalUser()
        {
            if (string.IsNullOrWhiteSpace(ViewModel.SigninEmail) || string.IsNullOrWhiteSpace(ViewModel.SigninPassword))
            {
                MessageBox.Show("Invalid email/password", "MTGAHelper");
                return;
            }

            AccountResponse info = Api.ValidateLocalUser(ViewModel.SigninEmail, ViewModel.SigninPassword);

            if (info == null)
            {
                MessageBox.Show("Cannot sign-in", "MTGAHelper");
            }
            else if (info.IsAuthenticated)
            {
                var signinPassword = "";

                if (ViewModel.RememberPassword)
                {
                    string salt = Api.GetAccountSalt();
                    signinPassword = PasswordHasher.Hash(ViewModel.SigninPassword, salt);
                }

                // Set the email and password in the config file
                Config.SigninEmail = ViewModel.RememberEmail || ViewModel.RememberPassword ? ViewModel.SigninEmail : "";
                Config.SigninPassword = signinPassword;

                // Set the sign-in (which saves the config)
                SetSignedIn(info);
            }
            else
            {
                MessageBox.Show(info.Message, "MTGAHelper");
            }
        }

        /// <summary>
        /// Set the sign-in status with the provided account response
        /// </summary>
        /// <param name="account"></param>
        private void SetSignedIn(AccountResponse account)
        {
            Config.SigninProvider = account.Provider;

            Config.Save();

            ViewModel.SetSignedIn(account);

            Api.SetUserId(account.MtgaHelperUserId);

            if (ViewModel.CanUpload == false || ViewModel.Account.IsAuthenticated == false)
                return;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    ViewModel.WrapNetworkStatus(NetworkStatusEnum.Downloading, () =>
                    {
                        CollectionResponse collection = Api.GetCollection();
                        ViewModel.SetCollection(collection);
                        RefreshRareDraftingInfo();
                    });

                    UploadLogFile();
                }
                catch (HttpRequestException)
                {
                    ViewModel.SetProblemServerUnavailable();
                }
            });

        }

        /// <summary>
        /// Download the rare drafting info while setting the status blinker
        /// </summary>
        private void RefreshRareDraftingInfo()
        {
            ViewModel.WrapNetworkStatus(NetworkStatusEnum.Downloading, () =>
            {
                RareDraftingInfo = Api.GetRaredraftingInfo(ViewModel.Account.MtgaHelperUserId).Data;
            });
        }

        /// <summary>
        /// Show the options window as a dialog and handle changes
        /// </summary>
        private void ShowOptionsWindow()
        {
            try
            {
                // Create the window, set the owner and show as a dialog window (blocking execution until closed)
                OptionsWindow win = new OptionsWindow().Init(ViewModel, Config, DraftRatings, DraftHelperRunner, ViewModel.Account.Email);
                win.Owner = this;
                win.ShowDialog();

                // Get a reference to the view model
                OptionsWindowVM ovm = win.OptionsViewModel;

                // Serialize the original config
                string origConfig = JsonConvert.SerializeObject(Config);

                // Perform a deep copy on the existing config
                var configModel = JsonConvert.DeserializeObject<ConfigModel>(origConfig);

                // Store the setting from the options window
                configModel.LogFilePath = ovm.LogFilePath.Trim();
                configModel.GameFilePath = ovm.GameFilePath.Trim();
                configModel.RunOnStartup = ovm.RunOnStartup;
                configModel.ShowOpponentCardsAuto = ovm.ShowOpponentCardsAuto;
                configModel.ShowOpponentCardsExternal = ovm.ShowOpponentCardsExternal;
                configModel.Minimize = ovm.Minimize;
                configModel.AutoShowHideForMatch = ovm.AutoShowHideForMatch;
                configModel.OrderLibraryCardsBy = ovm.OrderLibraryCardsBy; 
                configModel.ForceCardPopupSide = ovm.ForceCardPopupSide;
                configModel.ShowLimitedRatings = ovm.ShowLimitedRatings;
                configModel.LimitedRatingsSource = ovm.LimitedRatingsSource;
                configModel.ForceGameResolution = ovm.ForceGameResolution;
                configModel.GameResolutionIsPanoramic = ovm.GameResolutionIsPanoramic;
                configModel.GameResolution = ovm.GameResolution;

                // Serialize the new config
                string newConfig = JsonConvert.SerializeObject(configModel);

                // Check if the settings have changed
                if (origConfig != newConfig)
                {
                    // Get the original draft options
                    bool showRatings = Config.ShowLimitedRatings;
                    string ratingSource = Config.LimitedRatingsSource;

                    // Copy the properties and save the config
                    Config.CopyPropertiesFrom(configModel);
                    Config.Save();

                    // Handle changing the draft source during a draft
                    if (ViewModel.Context == WindowContext.Drafting &&
                        (Config.ShowLimitedRatings != showRatings || Config.LimitedRatingsSource != ratingSource))
                    {
                        if (Config.LimitedRatingsSource != ratingSource)
                            SetCardsDraft(ViewModel.DraftingVM.DraftInfoBuffered.DraftProgress);

                        DraftingControl.SetPopupRatingsSource(Config.ShowLimitedRatings, Config.LimitedRatingsSource);
                    }

                    // If the external option is disabled, hide the external window
                    if (!Config.ShowOpponentCardsExternal)
                        ViewModel.OpponentWindowVM.ShowHideWindow();
                    // If auto was just enabled, show the window (the playing state is checked internally)
                    else if (Config.ShowOpponentCardsAuto)
                        ViewModel.OpponentWindowVM.ShowHideWindow(true);

                    // If the height is minimized and they changed the option, restore the window
                    if (Config.Minimize != MinimizeOption.Height && ViewModel.IsHeightMinimized)
                        ViewModel.RestoreWindow();
                    
                    // Locate the resources from the new paths
                    ResourcesLocator.LocateLogFilePath(Config);
                    ResourcesLocator.LocateGameClientFilePath(Config);
                    FileMonitor.SetFilePath(configModel.LogFilePath);

                    // Set the start-up option
                    StartupManager.ManageRunOnStartup(configModel.RunOnStartup);

                    // Set the card order
                    ViewModel.OrderLibraryCardsBy = Config.OrderLibraryCardsBy == "Converted Mana Cost" ? CardsListOrder.ManaCost : CardsListOrder.DrawChance;
                }

                UpdateCardPopupPosition();
            }
            catch (Exception ex)
            {
                if (ex is InvalidEmailException)
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Stop);

                Log.Write(LogEventLevel.Error, ex, "Unexpected error:");
            }
        }

        private void UploadInfoToServer(string logToSend, Action callbackOnError = null)
        {
            if (ViewModel.CanUpload == false)
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
                            ViewModel.WrapNetworkStatus(NetworkStatusEnum.ProcessingLogFile, () =>
                            {
                                (result, errorId) = Reader.LoadFileContent(ViewModel.Account.MtgaHelperUserId, ms).Result;
                            });

                            if (result.CollectionByDate.Any(i => i.DateTime == default))
                                Api.LogErrorRemoteFile(ViewModel.Account.MtgaHelperUserId, logToSend, $"_NODATE_outputlog_{DateTime.Now:yyyyMMddHHmmss}.zip");

                            if (errorId.HasValue)
                            {
                                Api.LogErrorRemoteFile(ViewModel.Account.MtgaHelperUserId, logToSend, $"_parsererror_{errorId}_{DateTime.Now:yyyyMMddHHmmss}.zip");
                            }
                        }
                        catch (Exception ex)
                        {
                            if (logToSend != null)
                            {
                                Log.Error(ex, "Problem processing log piece ({logSize})", logToSend.Length);

                                Api.LogErrorRemoteFile(ViewModel.Account.MtgaHelperUserId, logToSend,
                                    $"_unknownError{DateTime.Now:yyyyMMddHHmmss}.zip");
                            }
                        }
                    }

                    ViewModel.WrapNetworkStatus(NetworkStatusEnum.Uploading, () =>
                    {

#if !DEBUG && !DEBUGWITHSERVER
                        CollectionResponse collection = Api.UploadOutputLogResult(ViewModel.Account.MtgaHelperUserId, result);
                        ViewModel.SetCollection(collection);
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
                    ViewModel.SetProblemServerUnavailable();
                }
            });
        }

        private void RefreshAccessToken()
        {
            try
            {
                if (ViewModel.Account.IsAuthenticated == false)
                    return;

                if (ViewModel.Account.Provider == null)
                {
                    Api.ValidateLocalUser(ViewModel.SigninEmail, ViewModel.SigninPassword);
                }
                else
                {
                    string token = null;
                    switch (ViewModel.Account.Provider)
                    {
                        case "Google":
                            token = TokenManager.GoogleRefresh();
                            break;
                        case "Facebook":
                            //token = tokenManager.FacebookRefresh();
                            // Always re-use the long-lived token
                            token = ViewModel.FacebookAccessToken;
                            break;
                    }

                    if (string.IsNullOrWhiteSpace(token) == false)
                        Api.ValidateExternalToken(ViewModel.Account.Provider, token);
                }
            }
            catch (Exception)
            {
                Log.Error("Remote server was not accessible for token refresh");
            }
        }

        private void UploadLogFragment()
        {
            var logToSend = FileMonitor.LogContentToSend.ToString();

            UploadInfoToServer(logToSend, () =>
            {
                NetworkStatusEnum flags = ViewModel.GetFlagsNetworkStatus();
                var activeStatus = Enum.GetValues(typeof(NetworkStatusEnum)).Cast<NetworkStatusEnum>()
                    .Where(i => i != NetworkStatusEnum.Ready)
                    .Where(i => flags.HasFlag(i))
                    .ToArray();

                // Only log when the problem is not that it just is already uploading
                if (activeStatus.Length != 1 || activeStatus[0] != NetworkStatusEnum.Uploading)
                    Log.Warning(
                        "FileSizeChangedNewText() Could not upload data. Status:{status} - Problems:{problems}",
                        string.Join(",", activeStatus), ViewModel.ProblemsList);
            });
        }

        private void UploadLogFile(Action callbackOnError = null)
        {
            if (File.Exists(Config.LogFilePath) && new FileInfo(Config.LogFilePath).Length > 0)
            {
                string logContent = LogFileZipper.ReadLogFile(Config.LogFilePath);

                UploadInfoToServer(logContent, callbackOnError);

                Reader.ResetPreparer();
            }
            else
                callbackOnError?.Invoke();
        }

        private void OnProcessMonitorStatusChanged(bool isRunning)
        {
            // Set the game running state
            ViewModel.IsGameRunning = isRunning;

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
            ViewModel.SetProblem(ProblemsFlags.LogFileNotFound, false);
            ViewModel.SizeOfLogToSend = FileMonitor.LogContentToSend.Length;

            ICollection<IMtgaOutputLogPartResult> messages;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(newText ?? "")))
            {
                messages = Reader.ProcessIntoMessagesAsync("local", ms).Result;
            }

            void GoHome()
            {
                ViewModel.SetMainWindowContext(WindowContext.Home);
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
                        ViewModel.SetProblem(ProblemsFlags.DetailedLogsDisabled, detLogRes.IsDetailedLoggingDisabled);
                        break;
                    case IResultCardPool msgCardPool:
                        {
                            var isLimitedCardPool = true;
                            if (msg is GetEventPlayerCourseV2Result playerCourse)
                            {
                                // Set the set drafted for the Human DraftHelper
                                var regexMatch = Regex.Match(playerCourse.Raw.payload.InternalEventName, "Draft_(.*?)_");
                                if (regexMatch.Success)
                                {
                                    DraftHelperRunner.Set = regexMatch.Groups[1].Value;
                                }

                                GetEventPlayerCourseV2Raw payload = playerCourse.Raw.payload;
                                if (payload.CurrentEventState == "PreMatch")
                                {
                                    ViewModel.DraftingVM.ResetDraftPicks(DraftHelperRunner.Set, false);
                                    //// TO SIMULATE HUMAN DRAFTING FROM A QUICKDRAFT LOG
                                    //MainWindowVM.DraftingVM.ResetDraftPicks(DraftHelperRunner.Set, true, Guid.NewGuid().ToString());
                                }

                                isLimitedCardPool = payload.InternalEventName.Contains("Draft") || payload.InternalEventName.Contains("Sealed");
                            }

                            if (isLimitedCardPool)
                            {
                                // Refresh the drafting window to show whole card pool
                                ViewModel.DraftingVM.ShowGlobalMTGAHelperSays = false;
                                SetCardsDraft(new DraftPickProgress(msgCardPool.CardPool));
                                ViewModel.SetMainWindowContext(WindowContext.Drafting);
                            }

                            break;
                        }
                    case IResultDraftPick msgDraftPack when msgDraftPack.Raw.payload.draftPack != null:
                        {
                            // Refresh the drafting window to show the new picks
                            ViewModel.DraftingVM.ShowGlobalMTGAHelperSays = true;
                            var draftInfo = Mapper.Map<DraftPickProgress>(msgDraftPack.Raw.payload);
                            SetCardsDraft(draftInfo);
                            //// TO SIMULATE HUMAN DRAFTING FROM A QUICKDRAFT LOG
                            //SetCardsDraft(draftInfo, true);
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
                                    ViewModel.SetInMatchStateBuffered(InGameTracker.State);
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
                    case GetActiveEventsV2Result getActiveEvents:
                        GoHome();
                        break;
                    //else if (msg is EventClaimPrizeResult claimPrize)
                    //{
                    //    // Clear the drafting window
                    //    SetCardsDraft(new int[0]);
                    //}
                    case MatchCreatedResult matchCreated:
                        ViewModel.SetMainWindowContext(WindowContext.Playing);
                        break;

                    case JoinPodmakingResult joinPodmaking:
                        var txt = joinPodmaking.Raw.request;
                        var match = Regex.Match(txt, @"\""(.*?Draft_(.*?)_.*?)\""");
                        if (match.Success)
                        {
                            var set = match.Groups[2].Value;
                            InitDraftHelperForHumanDraft(set);

                            // Hack to refresh properly the Pxpx combobox to P1p1 initially...
                            Task.Run(() =>
                            {
                                Task.Delay(500);
                                Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => ViewModel.DraftingVM.OnPropertyChanged(nameof(ViewModel.DraftingVM.PxpxItemSelected))));
                            });
                        }
                        break;
                    case GetPlayerInventoryResult _:
                    case GetPlayerCardsResult _:
                    case PostMatchUpdateResult _:
                    case RankUpdatedResult _:
                    case GetCombinedRankInfoResult _:
                    case InventoryUpdatedResult inventoryUpdated:
                        mustUpload = true;

                        if (ViewModel.DraftingVM.DraftProgressHuman?.PickedCards?.Count >= 42)
                        {
                            Log.Information("Must upload human draft:{NewLine}{humanDraft}", JsonConvert.SerializeObject(ViewModel.DraftingVM.DraftPicksHistory));
                        }
                        break;
                }

                if (ViewModel.Context == WindowContext.Playing)
                {
                    InGameTracker.ProcessMessage(msg);
                    ViewModel.SetInMatchStateBuffered(InGameTracker.State);
                }
            }

            if (mustUpload)
            {
                UploadLogFragment();
            }
        }

        private void InitDraftHelperForHumanDraft(string set)
        {
            ViewModel.SetMainWindowContext(WindowContext.Drafting);
            DraftHelperRunner.Set = set;
            ViewModel.DraftingVM.ResetDraftPicks(DraftHelperRunner.Set, true, Guid.NewGuid().ToString());
        }

        private void SetCardsDraft(DraftPickProgress draftInfo, bool isHuman = false)
        {
            var cardPool = draftInfo?.DraftPack;
            if (cardPool == null)
                return;

            var collection = ViewModel.Collection.Cards
                .Where(i => i.IdArena != 0)
                .ToDictionary(i => i.IdArena, i => i.Amount);

            var draftingInfo = DraftHelper.GetDraftPicksForCards(
                ViewModel.Account.MtgaHelperUserId,
                cardPool,
                draftInfo.PickedCards,
                Config.LimitedRatingsSource,
                collection,
                RareDraftingInfo);

            ViewModel.DraftingVM.SetCardsDraftBuffered(draftInfo, draftingInfo, isHuman);

            if (draftInfo.PickNumber == 0)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => DraftingControl.HideCardListWheeled()));
                ViewModel.DraftingVM.ShowCardListThatDidNotWheel = false;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCardPopupPosition();

            if (string.IsNullOrWhiteSpace(Config.SigninProvider) == false)
            {
                string token = Config.SigninProvider switch
                {
                    "Google" => await TokenManager.GoogleSignin(),
                    "Facebook" => TokenManager.FacebookSignin(this),
                    _ => null
                };

                ValidateExternalToken(Config.SigninProvider, token);
            }
            else if (string.IsNullOrWhiteSpace(Config.SigninEmail) == false)
            {
                if (string.IsNullOrWhiteSpace(Config.SigninPassword) == false)
                {
                    AccountResponse info = Api.AutoSigninLocalUser(Config.SigninEmail, Config.SigninPassword);
                    if (info == null)
                    {
                        MessageBox.Show("Cannot auto sign in the local account", "MTGAHelper");
                    }
                    else if (info.IsAuthenticated)
                    {
                        SetSignedIn(info);
                    }
                }
                else
                {
                    ViewModel.SigninEmail = Config.SigninEmail;
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

        public void UpdateCardPopupPosition()
        {
            if (double.IsNaN(Top) || double.IsNaN(Left) ||
                double.IsNaN(Width) || double.IsNaN(Height))
                return;

            var top = (int)Top;
            var left = (int)Left;
            var width = (int)Width;

            DraftingControl.SetCardPopupPosition(Config.ForceCardPopupSide, top, left, width);
            PlayingControl.SetCardPopupPosition(Config.ForceCardPopupSide, top, left, width);
        }

        public void RunDraftHelper()
        {
            try
            {
                if (ViewModel.DraftingVM.PxpxItemSelected == null)
                    ViewModel.DraftingVM.PxpxItemSelected = ViewModel.DraftingVM.PxpxItems.FirstOrDefault();

                // Validate all the requirements to run draft helper (including taking the screenshot)
                ViewModel.MinimizeWindow();
                DraftHelperRunnerValidationResultEnum validation = DraftHelperRunner.Validate(ViewModel.Account.Email);
                ViewModel.RestoreWindow();

                if (validation != DraftHelperRunnerValidationResultEnum.Success)
                {
                    var text = validation switch
                    {
                        DraftHelperRunnerValidationResultEnum.SetMissing =>
                        "DraftHelper doesn't know which set to draft",
                        DraftHelperRunnerValidationResultEnum.UnknownConfigResolution =>
                        "Impossible to determing the DraftHelper configuration for your game resolution",
                        _ => "Unknown error",
                    };
                    MessageBox.Show(text, "Problem", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }

                Visibility windowCardsThatDidNotWheelVisibility = DraftingControl.WindowCardsThatDidNotWheel.Visibility;
                DraftingControl.WindowCardsThatDidNotWheel.Visibility = Visibility.Collapsed;

                var draftHelperOutput = DraftHelperRunner.Run(ViewModel.DraftingVM.PxpxCardsNumber, Config.LimitedRatingsSource);
                var cardIds = draftHelperOutput
                        .Select(i => i.CardId)
                        .Where(i => i != 0)
                        .ToArray();

                SetCardsDraft(new DraftPickProgress(cardIds), true);

                UpdateCardPopupPosition();
                DraftingControl.WindowCardsThatDidNotWheel.Visibility = windowCardsThatDidNotWheelVisibility;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error in RunDraftHelper");
                Debugger.Break();
            }
        }
    }
}
