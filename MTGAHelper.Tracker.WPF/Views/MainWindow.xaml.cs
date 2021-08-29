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
using MTGAHelper.Entity.MtgaOutputLog;
using MTGAHelper.Lib;
using MTGAHelper.Lib.OutputLogParser;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Lib.OutputLogParser.Models;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;
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

namespace MTGAHelper.Tracker.WPF.Views
{
    public partial class MainWindow
    {
        /// <summary>
        /// Primary View Model
        /// </summary>
        public MainWindowVM ViewModel { get; }

        private ServerApiCaller Api => ViewModel.Api;
        private MtgaResourcesLocator ResourcesLocator => ViewModel.ResourcesLocator;
        private FileMonitor FileMonitor => ViewModel.FileMonitor;
        private ReaderMtgaOutputLog Reader => ViewModel.ReaderMtgaOutputLog;
        private ExternalProviderTokenManager TokenManager => ViewModel.TokenManager;

        private readonly IMapper mapper;
        private ICollection<CardCompareInfo> RareDraftingInfo;

        //private IList<int> lastCardsAetherized = new int[0];
        private Guid currentCourseId;

        private ICollection<EventGetCourseRaw> courses;

        private InGameTracker2 InGameTracker => ViewModel.InMatchTracker;

        /// <summary>
        /// Complete constructor
        /// </summary>
        public MainWindow(MainWindowVM viewModel)
        {
            mapper = viewModel.Mapper;
            ViewModel = viewModel;

            // Set the main window view model actions
            ViewModel.UploadLogAction = UploadLogAction;
            ViewModel.ShowOptionsAction = ShowOptionsWindow;
            ViewModel.LaunchArenaAction = LaunchArenaAction;
            ViewModel.ValidateUserAction = ValidateUserAction;

            // Set the problem action
            ResourcesLocator.SetProblem = ViewModel.SetProblem;

            // Locate the log file
            ResourcesLocator.LocateLogFilePath(ViewModel.Config);

            // Locate the game path
            ResourcesLocator.LocateGameClientFilePath(ViewModel.Config);

            FileMonitor.OnFileSizeChangedNewText += OnFileSizeChangedNewText;
            FileMonitor.SetFilePath(ViewModel.Config.LogFilePath);

            // Set the data context to the view model
            DataContext = ViewModel;

            InitializeComponent();

            PlayingControl.Init(ViewModel);
            DraftingControl.Init(ViewModel.DraftingVM, ViewModel.Config.LimitedRatingsSource);
            DraftingControl.SetPopupRatingsSource(ViewModel.Config.ShowLimitedRatings, ViewModel.Config.LimitedRatingsSource);

            // Set the process monitor status changed action
            viewModel.ProcessMonitor.OnProcessMonitorStatusChanged = OnProcessMonitorStatusChanged;

            // Start the process monitor without awaiting the task completion
            _ = viewModel.ProcessMonitor.Start(new System.Threading.CancellationToken());

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
                if (ViewModel.Config?.GameFilePath == null)
                    return;

                // Check if the process is already running
                if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(ViewModel.Config.GameFilePath)).Length > 0)
                {
                    MessageBox.Show("Game is already running.", "MTGAHelper");
                    return;
                }

                // Modify the path to point to the launcher
                string launcherPath = ViewModel.Config.GameFilePath.Replace("MTGA.exe", "MTGALauncher/MTGALauncher.exe");

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
                    signinPassword = ViewModel.PasswordHasher.Hash(ViewModel.SigninPassword, salt);
                }

                // Set the email and password in the Config file
                ViewModel.Config.SigninEmail = ViewModel.RememberEmail || ViewModel.RememberPassword ? ViewModel.SigninEmail : "";
                ViewModel.Config.SigninPassword = signinPassword;

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
            ViewModel.Config.SigninProvider = account.Provider;

            ViewModel.Config.Save();

            ViewModel.SetSignedIn(account);

            Api.SetUserId(account.MtgaHelperUserId);
            RefreshCustomRatingsFromServer();

            if (ViewModel.Account.IsAuthenticated == false)
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

                    if (ViewModel.CanUpload)
                        UploadLogFile();
                }
                catch (HttpRequestException)
                {
                    ViewModel.SetProblemServerUnavailable();
                }
            });
        }

        private void RefreshCustomRatingsFromServer()
        {
            var ratingsRaw = Api.GetCustomDraftRatings();
            var ratings = ratingsRaw
                .Select(i => new CustomDraftRating
                {
                    Set = i.Card.Set,
                    Name = i.Card.Name,
                    Note = i.Note,
                    Rating = i.Rating
                });

            ViewModel.DraftingVM.CustomRatingsBySetThenCardName = ratings
                .GroupBy(i => i.Set)
                .ToDictionary(i => i.Key, i => i.ToDictionary(i => i.Name, i => i));
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
                var optionsVM = mapper.Map<OptionsWindowVM>(ViewModel.Config);

                optionsVM.Sets = ViewModel.Sets.ToDictionary(i => i.Code, i => i);
                optionsVM.DraftRatings = ViewModel.DraftRatings;
                optionsVM.LimitedRatingsSource = optionsVM.LimitedRatingsSourcesDict.First(i => i.Key == optionsVM.LimitedRatingsSource.Key);

                OptionsWindow win = new OptionsWindow().Init(ViewModel, optionsVM);
                win.Owner = this;
                win.ShowDialog();

                // Get a reference to the view model
                OptionsWindowVM ovm = win.OptionsViewModel;

                // Serialize the original config
                string origConfig = JsonConvert.SerializeObject(ViewModel.Config);

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
                configModel.LimitedRatingsSource = ovm.LimitedRatingsSource.Key;
                configModel.ForceGameResolution = ovm.ForceGameResolution;
                //configModel.GameResolutionBlackBorders = ovm.GameResolutionBlackBorders;
                configModel.GameResolution = ovm.GameResolution;
                configModel.ShowAllDraftRatings = ovm.ShowAllDraftRatings;

                // Serialize the new config
                string newConfig = JsonConvert.SerializeObject(configModel);

                // Check if the settings have changed
                if (origConfig != newConfig)
                {
                    var showLimitedRatingsChanged = configModel.ShowLimitedRatings != ViewModel.Config.ShowLimitedRatings;
                    var limitedRatingsSourceChanged = configModel.LimitedRatingsSource != ViewModel.Config.LimitedRatingsSource;

                    // Copy the properties and save the config
                    ViewModel.Config.CopyPropertiesFrom(configModel);
                    ViewModel.Config.Save();

                    // Refresh custom ratings with latest from server
                    if (ViewModel.Config.LimitedRatingsSource == Constants.LIMITEDRATINGS_SOURCE_CUSTOM)
                    {
                        RefreshCustomRatingsFromServer();
                    }

                    // Handle changing the draft source during a draft
                    if (ViewModel.Context == WindowContext.Drafting && (showLimitedRatingsChanged || limitedRatingsSourceChanged))
                    {
                        ViewModel.DraftingVM.LimitedRatingsSource = ViewModel.Config.LimitedRatingsSource;

                        if (limitedRatingsSourceChanged)
                            SetCardsDraft(ViewModel.DraftingVM.DraftInfoBuffered.DraftProgress);

                        DraftingControl.SetPopupRatingsSource(ViewModel.Config.ShowLimitedRatings, ViewModel.Config.LimitedRatingsSource);
                    }

                    // If the external option is disabled, hide the external window
                    if (!ViewModel.Config.ShowOpponentCardsExternal)
                        ViewModel.OpponentWindowVM.ShowHideWindow();
                    // If auto was just enabled, show the window (the playing state is checked internally)
                    else if (ViewModel.Config.ShowOpponentCardsAuto)
                        ViewModel.OpponentWindowVM.ShowHideWindow(true);

                    // If the height is minimized and they changed the option, restore the window
                    if (ViewModel.Config.Minimize != MinimizeOption.Height && ViewModel.IsHeightMinimized)
                        ViewModel.RestoreWindow();

                    // Locate the resources from the new paths
                    ResourcesLocator.LocateLogFilePath(ViewModel.Config);
                    ResourcesLocator.LocateGameClientFilePath(ViewModel.Config);
                    FileMonitor.SetFilePath(configModel.LogFilePath);

                    // Set the start-up option
                    ViewModel.StartupManager.ManageRunOnStartup(configModel.RunOnStartup);

                    // Set the card order
                    ViewModel.OrderLibraryCardsBy = ViewModel.Config.OrderLibraryCardsBy == "Converted Mana Cost" ? CardsListOrder.ManaCost : CardsListOrder.DrawChance;
                }

                UpdateCardPopupPosition();
            }
            catch (Exception ex)
            {
                //if (ex is InvalidEmailException)
                //    MessageBox.Show(ex.Message, "Not available", MessageBoxButton.OK, MessageBoxImage.Information);
                //else
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
                    //LogSplitter.GetLastUploadHash(logToSend);
                    //if (api.IsSameLastUploadHash(vm.Account.MtgaHelperUserId, uploadHash))
                    //{
                    //    vm.WrapNetworkStatus(NetworkStatusEnum.UpToDate, () => Task.Delay(5000).Wait());
                    //    return;
                    //}

                    OutputLogResult result = null;
                    OutputLogResult2 result2 = null;
                    Guid? errorId = null;
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(logToSend ?? "")))
                    {
                        try
                        {
                            ViewModel.WrapNetworkStatus(NetworkStatusEnum.ProcessingLogFile, () =>
                            {
                                (result, errorId, result2) = Reader.LoadFileContent(ViewModel.Account.MtgaHelperUserId, ms).Result;
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
                        CollectionResponse collection = Api.UploadOutputLogResult(ViewModel.Account.MtgaHelperUserId, result);
                        //Api.UploadOutputLogResult2(ViewModel.Account.MtgaHelperUserId, result2);

                        ViewModel.SetCollection(collection);
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
            if (File.Exists(ViewModel.Config.LogFilePath) && new FileInfo(ViewModel.Config.LogFilePath).Length > 0)
            {
                string logContent = LogFileZipper.ReadLogFile(ViewModel.Config.LogFilePath);

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
            {
                GoHome();
                UploadLogFile();
            }
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

        private void GoHome()
        {
            currentCourseId = default;
            ViewModel.SetMainWindowContext(WindowContext.Home);
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

                    case ICardPool msgCardPool:
                        {
                            var isLimitedCardPool = true;
                            if (msg is EventJoinRequestResult playerCourse)
                            {
                                // Set the set drafted for the Human DraftHelper
                                string set = null;
                                var regexMatch = Regex.Match(playerCourse.Raw.FetchPayload().EventName, "(?:Draft|Sealed)_(.*?)_");
                                if (regexMatch.Success)
                                {
                                    set = regexMatch.Groups[1].Value;
                                }

                                var payload = playerCourse.Raw.FetchPayload();
                                //if (payload.CurrentEventState == "PreMatch")
                                {
                                    RefreshCustomRatingsFromServer();
                                    ViewModel.DraftingVM.ResetDraftPicks(set);
                                }

                                isLimitedCardPool = payload.EventName.Contains("Draft") || payload.EventName.Contains("Sealed");
                            }

                            if (isLimitedCardPool)
                            {
                                //ViewModel.CheckAndDownloadThumbnails(msgCardPool.CardPool);

                                // Refresh the drafting window to show whole card pool
                                ViewModel.DraftingVM.ShowGlobalMTGAHelperSays = false;
                                SetCardsDraft(new DraftPickProgress(msgCardPool.CardPool));
                                ViewModel.SetMainWindowContext(WindowContext.Drafting);
                            }

                            break;
                        }

                    case EventGetCoursesResult eventGetCourses:
                        courses = eventGetCourses.Raw.Courses;
                        var currentCourse = eventGetCourses.Raw.Courses.FirstOrDefault(i => i.CourseId == currentCourseId);
                        if (currentCourse != default &&
                            (currentCourse.InternalEventName.Contains("Draft") || currentCourse.InternalEventName.Contains("Sealed"))
                        )
                        {
                            // Refresh the drafting window to show whole card pool
                            ViewModel.DraftingVM.ShowGlobalMTGAHelperSays = false;
                            SetCardsDraft(new DraftPickProgress(currentCourse.CardPool));
                            ViewModel.SetMainWindowContext(WindowContext.Drafting);
                        }
                        break;

                    case EventJoinResult eventJoin:
                        currentCourseId = eventJoin.Raw.FetchPayload().Changes.FirstOrDefault()?.SourceId ?? default;
                        break;

                    case DraftPickStatusResult msgDraftPack when msgDraftPack.Payload != null:
                        {
                            ViewModel.SetMainWindowContext(WindowContext.Drafting);

                            // Refresh the drafting window to show the new picks
                            ViewModel.DraftingVM.ShowGlobalMTGAHelperSays = true;
                            var draftInfo = mapper.Map<DraftPickProgress>(msgDraftPack.Payload);
                            SetCardsDraft(draftInfo);
                            break;
                        }
                    //case MatchGameRoomStateChangedEvent matchGameRoomStateChanged:
                    //    if (matchGameRoomStateChanged.gameRoomInfo.stateType == "MatchGameRoomStateType_MatchCompleted")
                    //    {
                    //        mustUpload = true;
                    //        GoHome();
                    //    }
                    //    break;
                    //case LogInfoRequestResult logInfoContainer:
                    //    {
                    //        var prms = logInfoContainer.RequestParams;
                    //        if (prms.messageName == "DuelScene.EndOfMatchReport" || prms.humanContext.Contains("Client changed scenes to Home"))
                    //        {
                    //            //// Trigger to upload the stored log content
                    //            //UploadLogFragment();
                    //            mustUpload = true;
                    //        }

                    //        // Change MainWindowContext
                    //        switch (prms.messageName)
                    //        {
                    //            case "Client.SceneChange":
                    //                dynamic context = prms.payloadObject.context?.ToString();   // SceneChange_PayloadObject
                    //                if (prms.humanContext.Contains("Client changed scene") &&
                    //                    context.Contains("Draft") == false &&
                    //                    context.Contains("Opening sealed boosters") == false &&
                    //                    context.Contains("Sealed") == false &&
                    //                    context != "deck builder")
                    //                {
                    //                    GoHome();
                    //                }
                    //                break;

                    //            case "DuelScene.EndOfMatchReport":
                    //                GoHome();
                    //                InGameTracker.Reset();
                    //                ViewModel.SetInMatchStateBuffered(InGameTracker.State);
                    //                break;
                    //        }

                    //        break;
                    //    }
                    case StateChangedResult stateChanged:
                        {
                            if (stateChanged.SignifiesMatchEnd)
                            {
                                mustUpload = true;
                                GoHome();
                            }
                            break;
                        }
                    case GetActiveEventsV2Result _:
                    case GetActiveEventsV3Result _:
                        GoHome();
                        break;

                    //case MatchCreatedResult _:
                    //    ViewModel.SetMainWindowContext(WindowContext.Playing);
                    //    break;

                    case ConnectRespResult connectResp:
                        //var distinctCards = connectResp.Raw.connectResp.deckMessage.deckCards.Union(connectResp.Raw.connectResp.deckMessage.sideboardCards ?? new List<int>()).Distinct().ToArray();
                        //ViewModel.CheckAndDownloadThumbnails(distinctCards);
                        ViewModel.SetMainWindowContext(WindowContext.Playing);
                        break;

                    //case JoinPodmakingResult joinPodmaking:
                    //    var txt = joinPodmaking.RequestParams.queueId;
                    //    var match = Regex.Match(txt, @"Draft_(.*?)_\d+");
                    //    if (match.Success)
                    //    {
                    //        var set = match.Groups[2].Value;

                    //        //InitDraftHelperForHumanDraft(set);
                    //        RefreshCustomRatingsFromServer();
                    //        ViewModel.DraftingVM.ResetDraftPicks(set);
                    //    }
                    //    break;

                    //case MakeHumanDraftPickResult humanDraftPick:
                    //    ViewModel.SetMainWindowContext(WindowContext.Drafting);
                    //    var cardId = humanDraftPick.RequestParams.cardId;
                    //    DraftingControl.PickCard(cardId);
                    //    break;

                    case DraftNotifyResult draftNotify:
                        ViewModel.SetMainWindowContext(WindowContext.Drafting);

                        // Refresh the drafting window to show the new picks
                        ViewModel.DraftingVM.ShowGlobalMTGAHelperSays = true;
                        var info = new DraftPickProgress
                        {
                            DraftId = draftNotify.Raw.draftId.ToString(),
                            PackNumber = draftNotify.Raw.SelfPack - 1,
                            PickNumber = draftNotify.Raw.SelfPick - 1,
                            DraftPack = draftNotify.Raw.PackCards.Split(',').Select(i => int.Parse(i.Trim())).ToList(),
                        };
                        SetCardsDraft(info);
                        break;

                    case GetPlayerInventoryResult _:
                    case GetPlayerCardsResult _:
                    case PostMatchUpdateResult _:
                    case RankUpdatedResult _:
                    case GetCombinedRankInfoResult _:
                        mustUpload = true;
                        break;

                    //case InventoryUpdatedResult inventoryUpdated:
                    //    mustUpload = true;

                    //    var aetherizedCards = inventoryUpdated.Raw.payload.updates.LastOrDefault()?.aetherizedCards;
                    //    if (aetherizedCards != null && aetherizedCards.Any())
                    //        lastCardsAetherized = aetherizedCards.Select(i => i.grpId).ToArray();

                    //    break;

                    case SceneChangeResult sceneChange:
                        if (sceneChange.Raw.context.Contains("Sealed") || sceneChange.Raw.context.Contains("Draft"))
                        {
                            // Refresh the drafting window to show whole card pool
                            currentCourse = courses.FirstOrDefault(i => sceneChange.Raw.context.Split('_').All(x => i.InternalEventName.Contains(x)));
                            ViewModel.DraftingVM.ShowGlobalMTGAHelperSays = false;
                            //SetCardsDraft(new DraftPickProgress(lastCardsAetherized));
                            SetCardsDraft(new DraftPickProgress(currentCourse.CardPool));
                            ViewModel.SetMainWindowContext(WindowContext.Drafting);
                        }
                        else if (sceneChange.Raw.toSceneName == "Home")
                            GoHome();
                        break;

                    case AuthenticateResponseResult authenticateResponse:
                        ViewModel.SetMainWindowContext(WindowContext.Playing);
                        InGameTracker.ProcessMessage(msg);
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

        private void SetCardsDraft(DraftPickProgress draftInfo)
        {
            var cardPool = draftInfo?.DraftPack;
            if (cardPool == null)
                return;

            var collection = ViewModel.Collection.Cards
                .Where(i => i.IdArena != 0)
                .ToDictionary(i => i.IdArena, i => i.Amount);

            var draftingInfo = ViewModel.DraftHelper.GetDraftPicksForCards(
                cardPool,
                draftInfo.PickedCards,
                ViewModel.Config.LimitedRatingsSource,
                collection,
                RareDraftingInfo,
                ViewModel.DraftingVM.CustomRatingsBySetThenCardName);

            ViewModel.DraftingVM.SetCardsDraftBuffered(draftInfo, draftingInfo);
            ViewModel.DraftingVM.DraftRatings = draftingInfo;

            if (draftInfo.PickNumber == 0)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() => DraftingControl.HideCardListWheeled()));
                ViewModel.DraftingVM.ShowCardListThatDidNotWheel = false;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCardPopupPosition();

            if (string.IsNullOrWhiteSpace(ViewModel.Config.SigninProvider) == false)
            {
                string token = ViewModel.Config.SigninProvider switch
                {
                    "Google" => await TokenManager.GoogleSignin(),
                    "Facebook" => TokenManager.FacebookSignin(this),
                    _ => null
                };

                ValidateExternalToken(ViewModel.Config.SigninProvider, token);
            }
            else if (string.IsNullOrWhiteSpace(ViewModel.Config.SigninEmail) == false)
            {
                if (string.IsNullOrWhiteSpace(ViewModel.Config.SigninPassword) == false)
                {
                    AccountResponse info = Api.AutoSigninLocalUser(ViewModel.Config.SigninEmail, ViewModel.Config.SigninPassword);
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
                    ViewModel.SigninEmail = ViewModel.Config.SigninEmail;
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

            DraftingControl.SetCardPopupPosition(ViewModel.Config.ForceCardPopupSide, top, left, width);
            PlayingControl.SetCardPopupPosition(ViewModel.Config.ForceCardPopupSide, top, left, width);
        }
    }
}