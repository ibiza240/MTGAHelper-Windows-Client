using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Business.Monitoring;
using MTGAHelper.Tracker.WPF.Models;
using MTGAHelper.Tracker.WPF.Views;
using MTGAHelper.Tracker.WPF.Views.Helpers;
using MTGAHelper.Web.UI.Model.Response.User;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class MainWindowVM : ObservableObject
    {
        StatusBlinker statusBlinker;

        NetworkStatusEnum networkStatusDisplayed;
        bool isGameRunning;
        bool isInitialSetupDone => Problems.HasFlag(ProblemsFlags.LogFileNotFound) == false && Problems.HasFlag(ProblemsFlags.InvalidUserId) == false;

        Dictionary<NetworkStatusEnum, string> dictStatus = new Dictionary<NetworkStatusEnum, string>
        {
            { NetworkStatusEnum.Ready, "Ready" },
            { NetworkStatusEnum.UpToDate, "Server data is up to date" },
            { NetworkStatusEnum.Uploading, "Uploading log file to server..." },
            { NetworkStatusEnum.Downloading, "Gathering data from server..." },
        };

        Dictionary<ProblemsFlags, string> dictProblems = new Dictionary<ProblemsFlags, string>
        {
            { ProblemsFlags.LogFileNotFound, "Log file not found" },
            { ProblemsFlags.InvalidUserId, "Invalid User Id" },
            { ProblemsFlags.ServerUnavailable, "Remote server unavailable" },
            { ProblemsFlags.GameClientFileNotFound, "MTGArena game not found" },
        };

        public bool IsInMatch { get; set; }
        public bool IsDrafting { get; set; }
        public int SizeOfLogToSend { get; set; }

        public bool IsUploading => statusBlinker.HasFlag(NetworkStatusEnum.Uploading);
        public bool CanUpload => isInitialSetupDone && IsUploading == false;

        #region Bindings
        public ObservableProperty<bool> AlwaysOnTop { get; set; } = new ObservableProperty<bool>(true);
        public CollectionResponse Collection { get; set; } = new CollectionResponse();
        public ProblemsFlags Problems { get; private set; } = ProblemsFlags.None;
        public int Opacity { get; set; } = 90;

        public ICollection<CardDraftPick> CardsDraft { get; set; } = new CardDraftPick[0];

        public ICollection<Card> CardsDeck { get; set; } = new Card[]
        {
            new Card { ImageUrl = "/Assets/Images/MTGA.png", Name = "Test 1" },
            new Card { ImageUrl = "https://www.gravatar.com/avatar/c41cb884edf04a01fa5fc5f5a7960637?s=328&d=identicon&r=PG", Name = "Test 2" },
        };

        // Read-only bindings
        public MainWindowContextEnum MainWindowContext
        {
            get
            {
                if (isInitialSetupDone)
                {
                    if (IsInMatch) return MainWindowContextEnum.Ready;
                    else if (IsDrafting) return MainWindowContextEnum.Ready;
                    else return MainWindowContextEnum.Ready;
                }
                else
                    return MainWindowContextEnum.Welcome;
            }
        }

        public string NetworkStatusDisplayed => dictStatus[networkStatusDisplayed] + (networkStatusDisplayed == NetworkStatusEnum.Ready ? $" ({SizeOfLogToSend})" : "");

        public ICollection<string> ProblemsList => Enum.GetValues(typeof(ProblemsFlags)).Cast<ProblemsFlags>()
            .Where(i => i != ProblemsFlags.None)
            .Where(i => i != ProblemsFlags.GameClientFileNotFound)
            .Where(i => Problems.HasFlag(i))
            .Select(i => dictProblems[i])
            .ToArray();

        public bool IsWorking => statusBlinker.HasFlag(NetworkStatusEnum.Uploading) || statusBlinker.HasFlag(NetworkStatusEnum.Downloading);
        public bool ShowLaunchMtgaGameClient => Problems.HasFlag(ProblemsFlags.GameClientFileNotFound) == false && isGameRunning == false;
        public double OpacityPct => Opacity / 100.0d;
        //public string Username => Collection.MtgaUserProfile.PlayerId;
        public string CardsOwned => $"{Collection.Cards.Sum(i => i.Amount).ToString("#,##0")} cards owned{CollectionDateAsOf}";
        string CollectionDateAsOf => string.IsNullOrWhiteSpace(Collection.CollectionDate) || Collection.CollectionDate.StartsWith("0001") ?
            string.Empty : $"{Environment.NewLine}as of {Collection.CollectionDate}";
        #endregion

        public MainWindowVM(StatusBlinker statusBlinker)
        {
            this.statusBlinker = statusBlinker;
            statusBlinker.EmitStatus += StatusBlinkerEmitStatus;
        }

        public NetworkStatusEnum GetFlagsNetworkStatus() => statusBlinker.GetFlags();

        internal void WrapNetworkStatus(NetworkStatusEnum status, Action workToDo)
        {
            statusBlinker.SetNetworkStatus(status, true);
            RaisePropertyChangedEvent(nameof(IsWorking));
            try
            {
                workToDo();
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

        public void ValidateUserId(string userId)
        {
            var isUserIdValid = Guid.TryParse(userId, out Guid g);
            SetProblem(ProblemsFlags.InvalidUserId, isUserIdValid == false);
            RaisePropertyChangedEvent(nameof(MainWindowContext));
        }

        public void SetCollection(CollectionResponse collectionResponse)
        {
            Collection = collectionResponse;
            RaisePropertyChangedEvent(nameof(CardsOwned));
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

        internal void SetLogContentNewText(string newText, int logContentSize)
        {
            SizeOfLogToSend = logContentSize;

            if (newText.Contains("<== Draft.MakePick"))
                IsDrafting = true;
            else if (newText.Contains("Client.SceneChange"))
                IsDrafting = false;

            if (newText.Contains("Event.MatchCreated"))
                IsInMatch = true;
            else if (newText.Contains("DuelScene.EndOfMatchReport"))
                IsInMatch = false;

            RaisePropertyChangedEvent(nameof(MainWindowContext));
        }

        internal void SetCardsDraft(ICollection<CardDraftPick> cards)
        {
            CardsDraft = cards;
            RaisePropertyChangedEvent(nameof(CardsDraft));
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
