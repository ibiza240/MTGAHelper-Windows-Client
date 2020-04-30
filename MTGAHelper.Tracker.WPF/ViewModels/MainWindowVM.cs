using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MTGAHelper.Entity;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Tracker.WPF.Tools;
using MTGAHelper.Web.Models.Response.Account;
using MTGAHelper.Web.UI.Model.Response.User;
using Serilog;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class MainWindowVM : ObservableObject
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="statusBlinker"></param>
        /// <param name="inMatchState"></param>
        /// <param name="config"></param>
        public MainWindowVM(StatusBlinker statusBlinker, InMatchTrackerStateVM inMatchState, ConfigModel config)
        {
            // Set the status blinker reference
            StatusBlinker = statusBlinker;

            // Subscribe to the blinker event
            statusBlinker.EmitStatus += StatusBlinkerEmitStatus;

            // Set the match state reference
            InMatchState = inMatchState;

            // Set the library order from the config
            OrderLibraryCardsBy = config.OrderLibraryCardsBy == "Converted Mana Cost"
                ? CardsListOrder.ManaCost
                : CardsListOrder.DrawChance;

            // Set the reference the config
            ConfigModel = config;

            // Create the opponent window view model
            OpponentWindowViewModel = new OpponentWindowViewModel("Opponent Cards", this);

            // Subscribe to property changes
            PropertyChanged += OnPropertyChanged;

            // Set the animated icon state
            AnimatedIcon = ConfigModel?.AnimatedIcon ?? false;

            // Set the initial window settings
            PositionTop = WindowSettings?.Position.Y ?? 0;
            PositionLeft = WindowSettings?.Position.X ?? 0;
            WindowOpacity = WindowSettings?.Opacity ?? 0.9;
            WindowTopmost = WindowSettings?.Topmost ?? true;

            // Set the initial window size
            WindowWidth = WindowSettings != null && WindowSettings.Size.X > double.Epsilon
                ? WindowSettings.Size.X
                : 340;
            WindowHeight = WindowSettings != null && WindowSettings.Size.Y > double.Epsilon
                ? WindowSettings.Size.Y
                : 500;

        }

        #endregion

        #region Public Two-Way Bindable Properties

        /// <summary>
        /// The main window context
        /// </summary>
        public WindowContext MainWindowContext
        {
            get => _MainWindowContext;
            set
            {
                SetField(ref _MainWindowContext, value, nameof(MainWindowContext));

                RaisePropertyChangedEvent(nameof(IsPlaying));
                RaisePropertyChangedEvent(nameof(IsPlayingOrDrafting));
            }
        }

        /// <summary>
        /// The displayed network status
        /// </summary>
        public NetworkStatusEnum NetworkStatus
        {
            get => _NetworkStatusDisplayed;
            set
            {
                SetField(ref _NetworkStatusDisplayed, value, nameof(NetworkStatus));

                RaisePropertyChangedEvent(nameof(NetworkStatusString));
                RaisePropertyChangedEvent(nameof(IsWorking));
            }
        }

        /// <summary>
        /// The visibility of the window
        /// </summary>
        public bool IsWindowVisible
        {
            get => _IsWindowVisible;
            set => SetField(ref _IsWindowVisible, value, nameof(IsWindowVisible));
        }

        /// <summary>
        /// The window state of the window
        /// </summary>
        public WindowState WindowState
        {
            get => _WindowState;
            set => SetField(ref _WindowState, value, nameof(WindowState));
        }

        /// <summary>
        /// Authentication Account
        /// </summary>
        public AccountResponse Account
        {
            get => _Account;
            set => SetField(ref _Account, value, nameof(Account));
        }

        /// <summary>
        /// The window position
        /// </summary>
        public double PositionTop
        {
            get => _PositionTop;
            set => SetField(ref _PositionTop, value, nameof(PositionTop));
        }

        /// <summary>
        /// The window position
        /// </summary>
        public double PositionLeft
        {
            get => _PositionLeft;
            set => SetField(ref _PositionLeft, value, nameof(PositionLeft));
        }

        /// <summary>
        /// The window dimension
        /// </summary>
        public double WindowWidth
        {
            get => _WindowWidth;
            set => SetField(ref _WindowWidth, value, nameof(WindowWidth));
        }

        /// <summary>
        /// The window dimension
        /// </summary>
        public double WindowHeight
        {
            get => _WindowHeight;
            set => SetField(ref _WindowHeight, value, nameof(WindowHeight));
        }

        /// <summary>
        /// The window transparency
        /// </summary>
        public double WindowOpacity
        {
            get => _WindowOpacity;
            set => SetField(ref _WindowOpacity, value, nameof(WindowOpacity));
        }

        /// <summary>
        /// The window topmost setting
        /// </summary>
        public bool WindowTopmost
        {
            get => _WindowTopmost;
            set => SetField(ref _WindowTopmost, value, nameof(WindowTopmost));
        }

        /// <summary>
        /// Whether the icon is animated
        /// </summary>
        public bool AnimatedIcon
        {
            get => _AnimatedIcon;
            set => SetField(ref _AnimatedIcon, value, nameof(AnimatedIcon));
        }

        #endregion

        #region Public One-Way Bindable Properties

        /// <summary>
        /// Whether the current state is Playing
        /// </summary>
        public bool IsPlaying => MainWindowContext == WindowContext.Playing;

        /// <summary>
        /// Whether the current state is Playing of Drafting
        /// </summary>
        public bool IsPlayingOrDrafting => MainWindowContext == WindowContext.Playing || MainWindowContext == WindowContext.Drafting;

        /// <summary>
        /// Whether the current state is Welcome
        /// </summary>
        public bool IsWelcome => MainWindowContext == WindowContext.Welcome;

        /// <summary>
        /// The displayed network status string
        /// </summary>
        public string NetworkStatusString => DictStatus[NetworkStatus] +
                                             (NetworkStatus == NetworkStatusEnum.Ready ? $" ({SizeOfLogToSend})" : "");

        /// <summary>
        /// Whether the network is currently working
        /// </summary>
        public bool IsWorking => StatusBlinker.HasFlag(NetworkStatusEnum.Uploading) ||
                                 StatusBlinker.HasFlag(NetworkStatusEnum.Downloading) ||
                                 StatusBlinker.HasFlag(NetworkStatusEnum.ProcessingLogFile);

        /// <summary>
        /// Whether the application is uploading
        /// </summary>
        public bool IsUploading => StatusBlinker.HasFlag(NetworkStatusEnum.Uploading);

        /// <summary>
        /// Whether the network is ready to upload
        /// </summary>
        public bool CanUpload => IsInitialSetupDone && IsUploading == false;

        /// <summary>
        /// Number of cards in users collection
        /// </summary>
        public string CardsOwned => $"{Collection.Cards.Sum(i => i.Amount):#,##0} cards owned{CollectionDateAsOf}";

        /// <summary>
        /// Application version
        /// </summary>
        public string Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        #endregion

        #region Public Properties

        /// <summary>
        /// The config model
        /// </summary>
        public ConfigModel ConfigModel { get; }

        /// <summary>
        /// The view model for the opponent window
        /// </summary>
        public OpponentWindowViewModel OpponentWindowViewModel { get; } 

        public CardsListOrder OrderLibraryCardsBy { get; set; }

        public int SizeOfLogToSend { get; set; }

        #endregion

        #region Private Backing Fields

        /// <summary>
        /// The main window context
        /// </summary>
        private WindowContext _MainWindowContext = WindowContext.Welcome;

        /// <summary>
        /// The displayed network status
        /// </summary>
        private NetworkStatusEnum _NetworkStatusDisplayed;

        /// <summary>
        /// The visibility of the window
        /// </summary>
        private bool _IsWindowVisible = true;

        /// <summary>
        /// The window state of the window
        /// </summary>
        private WindowState _WindowState = WindowState.Normal;

        /// <summary>
        /// Authentication Account
        /// </summary>
        private AccountResponse _Account = new AccountResponse();

        /// <summary>
        /// The window position
        /// </summary>
        private double _PositionTop;

        /// <summary>
        /// The window position
        /// </summary>
        private double _PositionLeft;

        /// <summary>
        /// The window dimension
        /// </summary>
        private double _WindowWidth;

        /// <summary>
        /// The window dimension
        /// </summary>
        private double _WindowHeight;

        /// <summary>
        /// The window transparency
        /// </summary>
        private double _WindowOpacity;

        /// <summary>
        /// The window topmost setting
        /// </summary>
        private bool _WindowTopmost;

        /// <summary>
        /// Whether the icon is animated
        /// </summary>
        private bool _AnimatedIcon;

        #endregion

        #region Private Fields

        /// <summary>
        /// Config Window Settings
        /// </summary>
        private WindowSettings WindowSettings => ConfigModel?.WindowSettings;

        private readonly StatusBlinker StatusBlinker;

        private bool IsGameRunning;

        private bool IsInitialSetupDone => Problems.HasFlag(ProblemsFlags.LogFileNotFound) == false &&
                                           Problems.HasFlag(ProblemsFlags.SigninRequired) == false;

        private readonly Dictionary<NetworkStatusEnum, string> DictStatus = new Dictionary<NetworkStatusEnum, string>
        {
            {NetworkStatusEnum.Ready, "Ready"},
            {NetworkStatusEnum.UpToDate, "Server data is up to date"},
            {NetworkStatusEnum.Uploading, "Uploading log file to server..."},
            {NetworkStatusEnum.Downloading, "Gathering data from server..."},
            {NetworkStatusEnum.ProcessingLogFile, "Processing log file..."},
        };

        private readonly Dictionary<ProblemsFlags, string> DictProblems = new Dictionary<ProblemsFlags, string>
        {
            {ProblemsFlags.LogFileNotFound, "Log file not found"},
            {ProblemsFlags.SigninRequired, "Sign-in required"},
            {ProblemsFlags.ServerUnavailable, "Remote server unavailable"},
            {ProblemsFlags.GameClientFileNotFound, "MTGArena game not found"},
            {ProblemsFlags.DetailedLogsDisabled, "Detailed Logs not enabled in MTGArena"},
        };

        #endregion

        #region Internal Methods

        /// <summary>
        /// Set the main window context
        /// </summary>
        /// <param name="newWindowContext"></param>
        internal void SetMainWindowContext(WindowContext newWindowContext)
        {
            // Check the config settings for the AutoHide option
            if (ConfigModel.AutoShowHideForMatch)
            {
                // If the context is Home, minimize the window
                if (newWindowContext == WindowContext.Home)
                {
                    MinimizeWindow();
                }
                else
                {
                    RestoreWindow();

                    // Bring the Arena process to the front
                    Process process = Process.GetProcessesByName("MTGA").FirstOrDefault();
                    if (process != null) Utilities.SetForegroundWindow(process.MainWindowHandle);
                }
            }

            // Confirm that the user has signed in
            if (!IsWelcome)
                MainWindowContext = newWindowContext;
        }

        /// <summary>
        /// Set the signed-in status
        /// </summary>
        /// <param name="account"></param>
        internal void SetSignedIn(AccountResponse account)
        {
            // Set the account
            Account = account;

            // Clear the sign-in required flag
            SetProblem(ProblemsFlags.SigninRequired, Account.IsAuthenticated == false);

            // If the account is authenticated, set the main context to Home
            if (Account.IsAuthenticated)
                MainWindowContext = WindowContext.Home;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handle property changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(PositionLeft):
                {
                    WindowSettings.Position.X = PositionLeft;
                    break;
                }
                case nameof(PositionTop):
                {
                    WindowSettings.Position.Y = PositionTop;
                    break;
                }
                case nameof(WindowWidth):
                {
                    WindowSettings.Size.X = WindowWidth;
                    break;
                }
                case nameof(WindowHeight):
                {
                    WindowSettings.Size.Y = WindowHeight;
                    break;
                }
                case nameof(WindowOpacity):
                {
                    WindowSettings.Opacity = WindowOpacity;
                    break;
                }
                case nameof(WindowTopmost):
                {
                    WindowSettings.Topmost = WindowTopmost;
                    break;
                }
                case nameof(AnimatedIcon):
                {
                    ConfigModel.AnimatedIcon = AnimatedIcon;
                    break;
                }
            }
        }

        #endregion

        #region Show Hide Card Images Command

        public ICommand ShowHideImagesCommand
        {
            get
            {
                return _ShowHideImagesCommand ??= new RelayCommand(param => ShowHideImages(), param => Can_ShowHideImages());
            }
        }

        private ICommand _ShowHideImagesCommand;

        private static bool Can_ShowHideImages()
        {
            return true;
        }

        private void ShowHideImages()
        {
            // Simple error check
            if (InMatchState == null) return;

            // Flip the compression state of the library
            InMatchState.MyLibrary.ShowImage = !InMatchState.MyLibrary.ShowImage;

            // Flip the compression state of the sideboard
            InMatchState.MySideboard.ShowImage = !InMatchState.MySideboard.ShowImage;

            // Flip the compression state of the opponent cards
            InMatchState.OpponentCardsSeen.ShowImage = !InMatchState.OpponentCardsSeen.ShowImage;
        }

        #endregion

        #region Minimize Window Command

        public ICommand MinimizeWindowCommand
        {
            get
            {
                return _MinimizeWindowCommand ??= new RelayCommand(param => MinimizeWindow(), param => Can_MinimizeWindow());
            }
        }

        private ICommand _MinimizeWindowCommand;

        private static bool Can_MinimizeWindow()
        {
            return true;
        }

        public void MinimizeWindow()
        {
            // Check if the option is set to minimize to tray
            if (ConfigModel.MinimizeToSystemTray)
            {
                // If so, just hide the main window
                IsWindowVisible = false;
            }
            else
            {
                // Otherwise, set the window state to minimized
                WindowState = WindowState.Minimized;
            }

            // Hide the opponent window
            OpponentWindowViewModel.IsWindowVisible = false;
        }

        #endregion

        #region Restore Window Command

        public ICommand RestoreWindowCommand
        {
            get
            {
                return _RestoreWindowCommand ??= new RelayCommand(param => RestoreWindow(), param => Can_RestoreWindow());
            }
        }

        private ICommand _RestoreWindowCommand;

        private static bool Can_RestoreWindow()
        {
            return true;
        }

        public void RestoreWindow()
        {
            // Check if the option is set to minimize to tray
            if (ConfigModel.MinimizeToSystemTray)
            {
                // If so, just show the window
                IsWindowVisible = true;
            }
            else
            {
                // Otherwise, set the window state to normal
                WindowState = WindowState.Normal;
            }
        }

        #endregion

        #region Exit Application Command

        public ICommand ExitApplicationCommand
        {
            get
            {
                return _ExitApplicationCommand ??= new RelayCommand(param => ExitApplication(), param => Can_ExitApplication());
            }
        }

        private ICommand _ExitApplicationCommand;

        private static bool Can_ExitApplication()
        {
            return true;
        }

        private static void ExitApplication()
        {
            Application.Current.Shutdown();
        }

        #endregion











        #region Originals

        public ObservableProperty<string> SigninEmail { get; set; } = new ObservableProperty<string>("");

        public string SigninPassword { get; set; }

        public string FacebookAccessToken { get; set; }

        public InMatchTrackerStateVM InMatchState { get; set; }

        public CollectionResponse Collection { get; set; } = new CollectionResponse();

        public ProblemsFlags Problems { get; private set; } = ProblemsFlags.None;

        public DraftingVM DraftingVM { get; set; } = new DraftingVM();

        public ICollection<string> ProblemsList => Enum.GetValues(typeof(ProblemsFlags)).Cast<ProblemsFlags>()
            .Where(i => i != ProblemsFlags.None)
            .Where(i => i != ProblemsFlags.GameClientFileNotFound)
            .Where(i => Problems.HasFlag(i))
            .Select(i => DictProblems[i])
            .ToArray();

        public bool ShowLaunchMtgaGameClient =>
            Problems.HasFlag(ProblemsFlags.GameClientFileNotFound) == false && IsGameRunning == false;

        private string CollectionDateAsOf =>
            string.IsNullOrWhiteSpace(Collection.CollectionDate) || Collection.CollectionDate.StartsWith("0001")
                ? string.Empty
                : $" as of {Collection.CollectionDate}";

        public NetworkStatusEnum GetFlagsNetworkStatus() => StatusBlinker.GetFlags();

        internal void WrapNetworkStatusInNewTask(NetworkStatusEnum status, Action workToDo)
        {
            Task.Factory.StartNew(() => { WrapNetworkStatus(status, workToDo); });
        }

        internal void WrapNetworkStatus(NetworkStatusEnum status, Action workToDo)
        {
            StatusBlinker.SetNetworkStatus(status, true);
            RaisePropertyChangedEvent(nameof(IsWorking));
            try
            {
                workToDo();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Managed error in WrapNetworkStatus:");
            }
            finally
            {
                StatusBlinker.SetNetworkStatus(status, false);
                RaisePropertyChangedEvent(nameof(IsWorking));
            }
        }

        internal void SetIsGameRunning(bool isGameRunning)
        {
            IsGameRunning = isGameRunning;
            RaisePropertyChangedEvent(nameof(ShowLaunchMtgaGameClient));
        }

        public void UnSetProblem(ProblemsFlags flag) => SetProblem(flag, false);

        public void SetProblem(ProblemsFlags flag, bool isActive = true)
        {
            if (isActive)
                Problems |= flag; // Set flag
            else
                Problems &= ~flag; // Remove flag

            RaisePropertyChangedEvent(nameof(ProblemsList));
            RaisePropertyChangedEvent(nameof(ShowLaunchMtgaGameClient));
        }

        public void SetCollection(CollectionResponse collectionResponse)
        {
            Collection = collectionResponse;
            RaisePropertyChangedEvent(nameof(CardsOwned));
            //RemoveWelcome();
        }

        internal void SetProblemServerUnavailable()
        {
            Task.Factory.StartNew(() =>
            {
                SetProblem(ProblemsFlags.ServerUnavailable);
                Task.Delay(5000).Wait();
                UnSetProblem(ProblemsFlags.ServerUnavailable);
            });
        }

        internal void SetCardsDraftBuffered(DraftPickProgress draftProgress, ICollection<CardDraftPickWpf> ratingsInfo)
        {
            DraftingVM.SetCardsDraftBuffered(draftProgress, ratingsInfo);
        }

        internal void SetCardsDraftFromBuffered()
        {
            DraftingVM.SetCardsDraftFromBuffered();
        }

        internal void SetInMatchStateBuffered(IInGameState state)
        {
            InMatchState.Init(OrderLibraryCardsBy);
            InMatchState.SetInMatchStateBuffered(state);
        }

        internal void SetCardsInMatchTrackingFromBuffered()
        {
            InMatchState.SetInMatchStateFromBuffered();
            //RaisePropertyChangedEvent(nameof(InMatchState));
        }

        private void StatusBlinkerEmitStatus(object sender, NetworkStatusEnum status)
        {
            NetworkStatus = status;
        }

        #endregion
    }
}
