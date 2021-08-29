using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib;
using MTGAHelper.Lib.OutputLogParser;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Business.Monitoring;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.Tools;
using MTGAHelper.Web.Models.Response.Account;
using MTGAHelper.Web.UI.Model.Response.User;
using Serilog;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public partial class MainWindowVM : BasicModel
    {
        public MainWindowVM(StatusBlinker statusBlinker, InMatchTrackerStateVM inMatchState, ConfigModel config, DraftingVM draftingVM,
            IMapper mapper,
            ICacheLoader<Dictionary<int, Set>> cacheSets,
            ProcessMonitor processMonitor,
            ServerApiCaller api,
            StartupShortcutManager startupManager,
            MtgaResourcesLocator resourcesLocator,
            FileMonitor fileMonitor,
            DraftCardsPicker draftHelper,
            ReaderMtgaOutputLog readerMtgaOutputLog,
            InGameTracker2 inMatchTracker,
            ExternalProviderTokenManager tokenManager,
            PasswordHasher passwordHasher,
            CacheSingleton<Dictionary<string, DraftRatings>> draftRatings,
            DraftHelperRunner draftHelperRunner,
            //IEmailProvider emailProvider,
            ICollection<Card> allCards,
            CardThumbnailDownloader cardThumbnailDownloader,
            ServerApiCaller serverApiCaller
            )
        {
            // Set the status blinker reference
            StatusBlinker = statusBlinker;
            // Set the network status emission handler
            StatusBlinker.EmitStatus += status => { NetworkStatus = status; };

            Sets = cacheSets.LoadData().Values.ToArray();
            InMatchState = inMatchState;
            Config = config;
            DraftingVM = draftingVM;
            Mapper = mapper;
            ProcessMonitor = processMonitor;
            Api = api;
            StartupManager = startupManager;
            ResourcesLocator = resourcesLocator;
            FileMonitor = fileMonitor;
            DraftHelper = draftHelper;
            ReaderMtgaOutputLog = readerMtgaOutputLog;
            InMatchTracker = inMatchTracker;
            InMatchState.GameEnded += OnGameEnded;
            InMatchState.OpponentCardsUpdated += OnOpponentCardsUpdated;

            TokenManager = tokenManager;
            PasswordHasher = passwordHasher;
            DraftRatings = draftRatings;
            DraftHelperRunner = draftHelperRunner;
            //this.emailProvider = emailProvider;
            AllCards = allCards;
            CardThumbnailDownloader = cardThumbnailDownloader;
            this.serverApiCaller = serverApiCaller;

            // Set the library order from the config
            OrderLibraryCardsBy = config.OrderLibraryCardsBy == "Converted Mana Cost"
                ? CardsListOrder.ManaCost
                : CardsListOrder.DrawChance;

            // Create the opponent window view model
            OpponentWindowVM = new OpponentWindowVM("Opponent Cards", this, serverApiCaller);

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

        private void OnOpponentCardsUpdated(object sender, EventArgs e)
        {
            if (IsHeightMinimized ||
                PlayingSelectedTabIndex != 3 ||
                PlayingOpponentSelectedTabIndex != 1 ||
                InMatchState.OpponentCardsSeen.CardCount < 3
                )
                return;

            OpponentWindowVM.RefreshDecksUsingCards(InMatchState.OpponentCardsSeen.Cards.Select(i => i.Name).ToArray());
        }

        public ConfigModel Config { get; }
        public OpponentWindowVM OpponentWindowVM { get; }
        public CardsListOrder OrderLibraryCardsBy { get; set; }
        public int SizeOfLogToSend { get; set; }
        public Action UploadLogAction { get; set; }
        public Action ShowOptionsAction { get; set; }
        public Action LaunchArenaAction { get; set; }
        public Action<object> ValidateUserAction { get; set; }

        public ICollection<string> ProblemsList => Enum.GetValues(typeof(ProblemsFlags)).Cast<ProblemsFlags>()
            .Where(i => i != ProblemsFlags.None)
            .Where(i => i != ProblemsFlags.GameClientFileNotFound)
            .Where(i => Problems.HasFlag(i))
            .Select(i => DictProblems[i])
            .ToArray();

        private WindowContext _Context = WindowContext.Welcome;
        private NetworkStatusEnum _NetworkStatusDisplayed;
        private bool _IsWindowVisible = true;
        private WindowState _WindowState = WindowState.Normal;
        private AccountResponse _Account = new AccountResponse();

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

        private bool IsUploading => StatusBlinker.HasFlag(NetworkStatusEnum.Uploading);
        private WindowSettings WindowSettings => Config?.WindowSettings;
        private readonly StatusBlinker StatusBlinker;
        private readonly CardThumbnailDownloader CardThumbnailDownloader;
        private readonly ServerApiCaller serverApiCaller;

        //readonly IEmailProvider emailProvider;

        private bool IsInitialSetupDone => Problems.HasFlag(ProblemsFlags.LogFileNotFound) == false &&
                                           Problems.HasFlag(ProblemsFlags.SigninRequired) == false;

        private Dictionary<NetworkStatusEnum, string> DictStatus { get; } =
            new Dictionary<NetworkStatusEnum, string>
            {
                {NetworkStatusEnum.Ready, "Ready"},
                {NetworkStatusEnum.UpToDate, "Server data is up to date"},
                {NetworkStatusEnum.Uploading, "Uploading log file to server..."},
                {NetworkStatusEnum.Downloading, "Gathering data from server..."},
                {NetworkStatusEnum.ProcessingLogFile, "Processing log file..."},
            };

        private Dictionary<ProblemsFlags, string> DictProblems { get; } =
            new Dictionary<ProblemsFlags, string>
            {
                {ProblemsFlags.LogFileNotFound, "Log file not found"},
                {ProblemsFlags.SigninRequired, "Sign-in required"},
                {ProblemsFlags.ServerUnavailable, "Remote server unavailable"},
                {ProblemsFlags.GameClientFileNotFound, "MTGArena game not found"},
                {ProblemsFlags.DetailedLogsDisabled, "Detailed Logs not enabled in MTGArena"},
            };

        internal void SetMainWindowContext(WindowContext newContext)
        {
            // Confirm that the user has signed-in before changing the context
            if (Context != WindowContext.Welcome && Context != newContext)
                Context = newContext;
        }

        internal void SetSignedIn(AccountResponse account)
        {
            Account = account;

            SetProblem(ProblemsFlags.SigninRequired, Account.IsAuthenticated == false);

            if (Account.IsAuthenticated)
            {
                Context = WindowContext.Home;
                //emailProvider.Email = account.Email;
            }
        }

        internal void SetProblem(ProblemsFlags flag, bool isActive)
        {
            if (isActive)
                Problems |= flag; // Set flag
            else
                Problems &= ~flag; // Remove flag

            OnPropertyChanged(nameof(ProblemsList));
            OnPropertyChanged(nameof(ShowLaunchMtgaGameClient));
        }

        internal void SetProblemServerUnavailable()
        {
            Task.Factory.StartNew(() =>
            {
                SetProblem(ProblemsFlags.ServerUnavailable, true);
                Task.Delay(5000).Wait();
                SetProblem(ProblemsFlags.ServerUnavailable, false);
            });
        }

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

        public string FacebookAccessToken { get; set; }
        public Set[] Sets { get; }
        public InMatchTrackerStateVM InMatchState { get; set; }

        public CollectionResponse Collection { get; set; } = new CollectionResponse();

        public ProblemsFlags Problems { get; private set; } = ProblemsFlags.None;

        public DraftingVM DraftingVM { get; }
        public IMapper Mapper { get; }
        public ProcessMonitor ProcessMonitor { get; }
        public ServerApiCaller Api { get; }
        public StartupShortcutManager StartupManager { get; }
        public MtgaResourcesLocator ResourcesLocator { get; }
        public FileMonitor FileMonitor { get; }
        public DraftCardsPicker DraftHelper { get; }
        public ReaderMtgaOutputLog ReaderMtgaOutputLog { get; }
        public InGameTracker2 InMatchTracker { get; }
        public ExternalProviderTokenManager TokenManager { get; }
        public PasswordHasher PasswordHasher { get; }
        public CacheSingleton<Dictionary<string, DraftRatings>> DraftRatings { get; }
        public DraftHelperRunner DraftHelperRunner { get; }
        public ICollection<Card> AllCards { get; }

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
            InMatchState.Init(OrderLibraryCardsBy, Config.CardListCollapsed);
            InMatchState.SetInMatchStateBuffered(state);
        }

        //internal void CheckAndDownloadThumbnails(ICollection<int> grpIds)
        //{
        //    CardThumbnailDownloader.CheckAndDownloadThumbnails(grpIds);
        //}

        private void OnGameEnded(object sender, EventArgs e)
        {
            OpponentWindowVM.CardList.ResetCards();
            OpponentWindowVM.ResetDecks();
        }
    }
}