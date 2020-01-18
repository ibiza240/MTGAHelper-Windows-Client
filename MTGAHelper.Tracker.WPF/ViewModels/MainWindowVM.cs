using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MTGAHelper.Entity;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Web.Models.Response.Account;
using MTGAHelper.Web.UI.Model.Response.User;
using Serilog;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class MainWindowVM : ObservableObject
    {
        readonly StatusBlinker statusBlinker;
        NetworkStatusEnum networkStatusDisplayed;
        bool isGameRunning;
        bool isInitialSetupDone => Problems.HasFlag(ProblemsFlags.LogFileNotFound) == false && Problems.HasFlag(ProblemsFlags.SigninRequired) == false;
        
        public CardsListOrder OrderLibraryCardsBy { get; set; }

        readonly Dictionary<NetworkStatusEnum, string> dictStatus = new Dictionary<NetworkStatusEnum, string>
        {
            { NetworkStatusEnum.Ready, "Ready" },
            { NetworkStatusEnum.UpToDate, "Server data is up to date" },
            { NetworkStatusEnum.Uploading, "Uploading log file to server..." },
            { NetworkStatusEnum.Downloading, "Gathering data from server..." },
            { NetworkStatusEnum.ProcessingLogFile, "Processing log file..." },
        };

        readonly Dictionary<ProblemsFlags, string> dictProblems = new Dictionary<ProblemsFlags, string>
        {
            { ProblemsFlags.LogFileNotFound, "Log file not found" },
            { ProblemsFlags.SigninRequired, "Sign-in required" },
            { ProblemsFlags.ServerUnavailable, "Remote server unavailable" },
            { ProblemsFlags.GameClientFileNotFound, "MTGArena game not found" },
        };

        //public bool IsInMatch { get; set; }
        //public bool IsDrafting { get; set; }
        public int SizeOfLogToSend { get; set; }

        public bool IsUploading => statusBlinker.HasFlag(NetworkStatusEnum.Uploading);
        public bool CanUpload => isInitialSetupDone && IsUploading == false;

        #region Bindings
        public AccountResponse Account { get; set; } = new AccountResponse();

        public ObservableProperty<string> SigninEmail { get; set; } = new ObservableProperty<string>("");
        public string SigninPassword { get; set; }
        public string FacebookAccessToken { get; set; }

        public InMatchTrackerStateVM InMatchState { get; set; }

        public ObservableProperty<bool> AlwaysOnTop { get; set; } = new ObservableProperty<bool>(true);
        public CollectionResponse Collection { get; set; } = new CollectionResponse();
        public ProblemsFlags Problems { get; private set; } = ProblemsFlags.None;
        public int Opacity { get; set; } = 90;

        public DraftingVM DraftingVM { get; set; } = new DraftingVM();

        //public ICollection<CardVM> CardsDeck { get; set; } = new CardVM[]
        //{
        //    new CardVM { ImageArtUrl = "/Assets/Images/MTGA.png", Name = "Test 1" },
        //    new CardVM { ImageArtUrl = "https://www.gravatar.com/avatar/c41cb884edf04a01fa5fc5f5a7960637?s=328&d=identicon&r=PG", Name = "Test 2" },
        //};

        public MainWindowContextEnum MainWindowContext { get; set; } = MainWindowContextEnum.Welcome;

        public string Version => System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        // Read-only bindings
        public string NetworkStatusDisplayed => dictStatus[networkStatusDisplayed] + (networkStatusDisplayed == NetworkStatusEnum.Ready ? $" ({SizeOfLogToSend})" : "");

        public ICollection<string> ProblemsList => Enum.GetValues(typeof(ProblemsFlags)).Cast<ProblemsFlags>()
            .Where(i => i != ProblemsFlags.None)
            .Where(i => i != ProblemsFlags.GameClientFileNotFound)
            .Where(i => Problems.HasFlag(i))
            .Select(i => dictProblems[i])
            .ToArray();

        public bool IsWorking => statusBlinker.HasFlag(NetworkStatusEnum.Uploading) || statusBlinker.HasFlag(NetworkStatusEnum.Downloading) || statusBlinker.HasFlag(NetworkStatusEnum.ProcessingLogFile);
        public bool ShowLaunchMtgaGameClient => Problems.HasFlag(ProblemsFlags.GameClientFileNotFound) == false && isGameRunning == false;
        public double OpacityPct => Opacity / 100.0d;
        //public string Username => Collection.MtgaUserProfile.PlayerId;
        public string CardsOwned => $"{Collection.Cards.Sum(i => i.Amount).ToString("#,##0")} cards owned{CollectionDateAsOf}";
        string CollectionDateAsOf => string.IsNullOrWhiteSpace(Collection.CollectionDate) || Collection.CollectionDate.StartsWith("0001") ?
            string.Empty : $" as of {Collection.CollectionDate}";
        #endregion

        public MainWindowVM(StatusBlinker statusBlinker, InMatchTrackerStateVM inMatchState, IOptionsMonitor<ConfigModelApp> config)
        {
            this.statusBlinker = statusBlinker;
            statusBlinker.EmitStatus += StatusBlinkerEmitStatus;

            this.InMatchState = inMatchState;
            this.OrderLibraryCardsBy = config.CurrentValue.OrderLibraryCardsBy == "Converted Mana Cost" ? CardsListOrder.Cmc : CardsListOrder.DrawChance;
        }

        public NetworkStatusEnum GetFlagsNetworkStatus() => statusBlinker.GetFlags();

        internal void WrapNetworkStatusInNewTask(NetworkStatusEnum status, Action workToDo)
        {
            Task.Factory.StartNew(() =>
            {
                WrapNetworkStatus(status, workToDo);
            });
        }

        internal void WrapNetworkStatus(NetworkStatusEnum status, Action workToDo)
        {
            statusBlinker.SetNetworkStatus(status, true);
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
                statusBlinker.SetNetworkStatus(status, false);
                RaisePropertyChangedEvent(nameof(IsWorking));
            }
        }

        internal void SetIsGameRunning(bool isGameRunning)
        {
            this.isGameRunning = isGameRunning;
            RaisePropertyChangedEvent(nameof(ShowLaunchMtgaGameClient));
        }

        internal void RefreshOpacity()
        {
            RaisePropertyChangedEvent(nameof(Opacity));
            RaisePropertyChangedEvent(nameof(OpacityPct));
        }

        public void SetProblem(ProblemsFlags flag, bool isActive)
        {
            if (isActive)
                Problems |= flag;  // Set flag
            else
                Problems &= ~flag;  // Remove flag

            RaisePropertyChangedEvent(nameof(ProblemsList));
            RaisePropertyChangedEvent(nameof(ShowLaunchMtgaGameClient));
        }

        internal void SetSignedIn(AccountResponse account)
        {
            Account = account;

            //void ValidateUserId(/*string userId*/)
            //{
            //var isUserIdValid = Guid.TryParse(userId, out Guid g);
            SetProblem(ProblemsFlags.SigninRequired, Account.IsAuthenticated == false);
            //RaisePropertyChangedEvent(nameof(MainWindowContext));

            //if (isUserIdValid)
            if (Account.IsAuthenticated)
                RemoveWelcome();
            //}

            RaisePropertyChangedEvent(nameof(Account));
        }

        public void SetCollection(CollectionResponse collectionResponse)
        {
            Collection = collectionResponse;
            RaisePropertyChangedEvent(nameof(CardsOwned));
            //RemoveWelcome();
        }

        public void RemoveWelcome()
        {
            if (MainWindowContext == MainWindowContextEnum.Welcome)
            {
                MainWindowContext = MainWindowContextEnum.Home;
                RaisePropertyChangedEvent(nameof(MainWindowContext));
            }
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

        internal void SetMainWindowContext(MainWindowContextEnum newWindowContext)
        {
            if (MainWindowContext != MainWindowContextEnum.Welcome)
            {
                MainWindowContext = newWindowContext;
                RaisePropertyChangedEvent(nameof(MainWindowContext));
            }
        }

        //internal void SetMainWindowContext(string newText, int logContentSize)
        //{
        //    SizeOfLogToSend = logContentSize;

        //    if (MainWindowContext != MainWindowContextEnum.Welcome)
        //    {
        //        if (newText.Contains("<== Draft.MakePick") || newText.Contains("<== Draft.DraftStatus"))
        //            MainWindowContext = MainWindowContextEnum.Drafting;
        //        //else if (newText.Contains("Client.SceneChange") || newText.Contains("Draft.Complete"))
        //        //    IsDrafting = false;

        //        if (newText.Contains("Event.MatchCreated"))
        //            MainWindowContext = MainWindowContextEnum.Playing;
        //        else if (newText.Contains("DuelScene.EndOfMatchReport"))
        //            MainWindowContext = MainWindowContextEnum.Home;
        //    }

        //    RaisePropertyChangedEvent(nameof(MainWindowContext));
        //}

        internal void SetCardsDraftBuffered(DraftPickProgress draftProgress, ICollection<CardDraftPickWpf> ratingsInfo)
        {
            DraftingVM.SetCardsDraftBuffered(draftProgress, ratingsInfo);
        }
        internal void SetCardsDraftFromBuffered()
        {
            DraftingVM.SetCardsDraftFromBuffered();
        }

        internal void SetInMatchStateBuffered(InGameTrackerState state)
        {
            InMatchState.Init(OrderLibraryCardsBy);
            InMatchState.SetInMatchStateBuffered(state);
        }
        internal void SetCardsInMatchTrackingFromBuffered()
        {
            InMatchState.SetInMatchStateFromBuffered();
            //RaisePropertyChangedEvent(nameof(InMatchState));
        }

        #region Event handlers
        void StatusBlinkerEmitStatus(object sender, NetworkStatusEnum status)
        {
            networkStatusDisplayed = status;
            RaisePropertyChangedEvent(nameof(NetworkStatusDisplayed));
            RaisePropertyChangedEvent(nameof(IsWorking));
        }
        #endregion
    }
}
