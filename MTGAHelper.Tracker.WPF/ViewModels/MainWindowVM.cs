using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.Tools;
using MTGAHelper.Web.Models.Response.Account;
using MTGAHelper.Web.UI.Model.Response.User;
using Serilog;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public partial class MainWindowVM : BasicModel
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
            // Set the reference the config
            Config = config;

            // Set the status blinker reference
            StatusBlinker = statusBlinker;

            // Set the network status emission handler
            StatusBlinker.EmitStatus += status => { NetworkStatus = status; };

            // Set the match state reference
            InMatchState = inMatchState;

            // Set the initial state of the compression
            SetCompressedCardList(Config.CardListCollapsed);

            // Set the library order from the config
            OrderLibraryCardsBy = config.OrderLibraryCardsBy == "Converted Mana Cost"
                ? CardsListOrder.ManaCost
                : CardsListOrder.DrawChance;

            // Create the opponent window view model
            OpponentWindowVM = new OpponentWindowVM("Opponent Cards", this);

            // Subscribe to property changes
            PropertyChanged += OnPropertyChanged;

            // Set the animated icon state
            AnimatedIcon = Config?.AnimatedIcon ?? false;

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

        #region Public Properties

        /// <summary>
        /// The config model
        /// </summary>
        public ConfigModel Config { get; }

        /// <summary>
        /// The view model for the opponent window
        /// </summary>
        public OpponentWindowVM OpponentWindowVM { get; } 

        /// <summary>
        /// Card list sort type
        /// </summary>
        public CardsListOrder OrderLibraryCardsBy { get; set; }

        /// <summary>
        /// Size of log file to upload
        /// </summary>
        public int SizeOfLogToSend { get; set; }

        /// <summary>
        /// Action for uploading the log file
        /// </summary>
        public Action UploadLogAction { get; set; }

        /// <summary>
        /// Action for showing the options
        /// </summary>
        public Action ShowOptionsAction { get; set; }

        /// <summary>
        /// Action for launching Arena
        /// </summary>
        public Action LaunchArenaAction { get; set; }

        /// <summary>
        /// Action for validating user
        /// </summary>
        public Action<object> ValidateUserAction { get; set; }

        /// <summary>
        /// List of problems
        /// </summary>
        public ICollection<string> ProblemsList => Enum.GetValues(typeof(ProblemsFlags)).Cast<ProblemsFlags>()
            .Where(i => i != ProblemsFlags.None)
            .Where(i => i != ProblemsFlags.GameClientFileNotFound)
            .Where(i => Problems.HasFlag(i))
            .Select(i => DictProblems[i])
            .ToArray();

        #endregion

        #region Private Backing Fields

        /// <summary>
        /// The main window context
        /// </summary>
        private WindowContext _Context = WindowContext.Welcome;

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

        /// <summary>
        /// Whether the game is running
        /// </summary>
        private bool _IsGameRunning;

        /// <summary>
        /// The users sign-in email
        /// </summary>
        private string _SigninEmail;

        /// <summary>
        /// Whether to remember the email
        /// </summary>
        private bool _RememberEmail;

        /// <summary>
        /// The users sign-in password
        /// </summary>
        private string _SigninPassword;

        /// <summary>
        /// Whether to remember the password
        /// </summary>
        private bool _RememberPassword;

        /// <summary>
        /// Whether the window has been height minimized
        /// </summary>
        private bool _IsHeightMinimized;

        #endregion

        #region Private Fields

        /// <summary>
        /// The displayed network status
        /// </summary>
        private NetworkStatusEnum NetworkStatus
        {
            get => _NetworkStatusDisplayed;
            set
            {
                SetField(ref _NetworkStatusDisplayed, value, nameof(NetworkStatus));

                OnPropertyChanged(nameof(NetworkStatusString));
                OnPropertyChanged(nameof(IsWorking));
            }
        }

        /// <summary>
        /// Whether the application is uploading
        /// </summary>
        private bool IsUploading => StatusBlinker.HasFlag(NetworkStatusEnum.Uploading);

        /// <summary>
        /// Config Window Settings
        /// </summary>
        private WindowSettings WindowSettings => Config?.WindowSettings;

        /// <summary>
        /// The network status indicator
        /// </summary>
        private readonly StatusBlinker StatusBlinker;

        /// <summary>
        /// Whether the initial setup is complete
        /// </summary>
        private bool IsInitialSetupDone => Problems.HasFlag(ProblemsFlags.LogFileNotFound) == false &&
                                           Problems.HasFlag(ProblemsFlags.SigninRequired) == false;

        /// <summary>
        /// Dictionary of network status phrases
        /// </summary>
        private Dictionary<NetworkStatusEnum, string> DictStatus { get; } =
            new Dictionary<NetworkStatusEnum, string>
            {
                {NetworkStatusEnum.Ready, "Ready"},
                {NetworkStatusEnum.UpToDate, "Server data is up to date"},
                {NetworkStatusEnum.Uploading, "Uploading log file to server..."},
                {NetworkStatusEnum.Downloading, "Gathering data from server..."},
                {NetworkStatusEnum.ProcessingLogFile, "Processing log file..."},
            };

        /// <summary>
        /// Dictionary of problem flag phrases
        /// </summary>
        private Dictionary<ProblemsFlags, string> DictProblems { get; } =
            new Dictionary<ProblemsFlags, string>
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
        /// <param name="newContext"></param>
        internal void SetMainWindowContext(WindowContext newContext)
        {
            // Confirm that the user has signed-in before changing the context
            if (Context != WindowContext.Welcome)
                Context = newContext;
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
                Context = WindowContext.Home;
        }

        /// <summary>
        /// Set a problem flag as active or inactive
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="isActive"></param>
        internal void SetProblem(ProblemsFlags flag, bool isActive)
        {
            if (isActive)
                Problems |= flag; // Set flag
            else
                Problems &= ~flag; // Remove flag

            OnPropertyChanged(nameof(ProblemsList));
            OnPropertyChanged(nameof(ShowLaunchMtgaGameClient));
        }

        /// <summary>
        /// Set a server problem flag and clear after 5 seconds
        /// </summary>
        internal void SetProblemServerUnavailable()
        {
            Task.Factory.StartNew(() =>
            {
                SetProblem(ProblemsFlags.ServerUnavailable, true);
                Task.Delay(5000).Wait();
                SetProblem(ProblemsFlags.ServerUnavailable, false);
            });
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
                    Config.AnimatedIcon = AnimatedIcon;
                    break;
                }
                case nameof(Context):
                {
                    // Handle context changes related to window state if AutoShowHide is enabled
                    if (Config.AutoShowHideForMatch)
                    {
                        // If the context is Home, hide the window, otherwise restore it
                        if (Context == WindowContext.Home)
                            MinimizeWindow();
                        else
                            RestoreWindow();
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Set the compression state of the match state card lists
        /// </summary>
        /// <param name="compressed"></param>
        private void SetCompressedCardList(bool compressed)
        {
            // Simple error check
            if (InMatchState == null) return;

            // Set the compression state of the library
            InMatchState.MyLibrary.ShowImage = !compressed;

            // Set the compression state of the sideboard
            InMatchState.MySideboard.ShowImage = !compressed;

            // Set the compression state of the opponent cards
            InMatchState.OpponentCardsSeen.ShowImage = !compressed;
        }

        #endregion

        #region Originals

        public string FacebookAccessToken { get; set; }

        public InMatchTrackerStateVM InMatchState { get; set; }

        public CollectionResponse Collection { get; set; } = new CollectionResponse();

        public ProblemsFlags Problems { get; private set; } = ProblemsFlags.None;

        public DraftingVM DraftingVM { get; set; } = new DraftingVM();

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

            OnPropertyChanged(nameof(IsWorking));

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
                OnPropertyChanged(nameof(IsWorking));
            }
        }

        public void SetCollection(CollectionResponse collectionResponse)
        {
            Collection = collectionResponse;
            OnPropertyChanged(nameof(CardsOwned));
            //RemoveWelcome();
        }

        internal void SetInMatchStateBuffered(IInGameState state)
        {
            InMatchState.Init(OrderLibraryCardsBy);
            InMatchState.SetInMatchStateBuffered(state);
        }

        #endregion
    }
}
