using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Entity.GameEvents;
using MTGAHelper.Entity.MtgaOutputLog;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Tracker.WPF.Business;
using MTGAHelper.Tracker.WPF.Tools;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class ManaSource
    {
        public string Info { get; set; }
        public string InfoIncludingSpellLands { get; set; }
    }

    public class InMatchTrackerStateVM : BasicModel
    {
        public event EventHandler GameEnded;

        public event EventHandler OpponentCardsUpdated;

        //enum CardListTypeEnum
        //{
        //    Unknown,
        //    MyLibrary,
        //    OpponentCards,
        //    MyFullDeck,
        //}

        private int PriorityPlayer;
        private bool MustReset;
        private IInGameState StateBuffered;
        private bool UpdateCardsInMatchTrackingBuffered;

        private readonly IMapper mapper;
        private readonly CardThumbnailDownloader cardThumbnailDownloader;
        private readonly Dictionary<int, Card> dictAllCards;
        private readonly object LockCardsInMatchTracking = new object();

        public int TurnNumber { get; private set; }
        public string OnThePlay { get; private set; }
        public CardsListVM MyLibrary { get; }
        public CardsListVM MyLibraryWithoutLands { get; }
        public CardsListVM OpponentCardsSeen { get; }
        public CardsListVM OpponentCardsPrevGames { get; }
        public ObservableCollection<ActionLogEntry> ActionLog { get; } = new ObservableCollection<ActionLogEntry>();
        public CardsListVM MySideboard { get; }
        public string LibraryLandCurrentAndTotal => $"{MyLibrary.LandCount} / {MyLibrary.TotalLandsInitial}";
        public float LibraryDrawLandPct => MyLibrary.DrawLandPct;

        public int LibraryCardsCount => MyLibrary.CardCount;
        public int SideboardCardsCount => MySideboard.CardCount;
        public int OpponentCardsSeenCount => OpponentCardsSeen.CardCount;
        public int OpponentCardsPrevGamesCount => OpponentCardsPrevGames.CardCount;
        public string OpponentScreenName { get; set; }

        public PlayerTimerVM TimerMe { get; set; } = new PlayerTimerVM();
        public PlayerTimerVM TimerOpponent { get; set; } = new PlayerTimerVM();

        //public CardsListVM FullDeck { get; set; } = new CardsListVM(false);

        //private bool _SplitLands;
        //public bool SplitLands
        //{
        //    get => _SplitLands;
        //    set => SetField(ref _SplitLands, value, nameof(SplitLands));
        //}

        public Dictionary<string, ManaSource> LibraryDrawManaBySource
        {
            get => _LibraryDrawManaBySource;
            set => SetField(ref _LibraryDrawManaBySource, value, nameof(LibraryDrawManaBySource));
        }

        private bool _ShowCardLands = true;

        public bool ShowLands
        {
            get => _ShowCardLands;
            set => SetField(ref _ShowCardLands, value, nameof(ShowLands));
        }

        private Dictionary<string, ManaSource> _LibraryDrawManaBySource = new Dictionary<string, ManaSource>();

        private static bool Can_ShowHideLands() => true;

        private void ShowHideLands() => ShowLands = !ShowLands;

        private ICommand _ShowHideLandsCommand;

        public ICommand ShowHideLandsCommand
        {
            get
            {
                return _ShowHideLandsCommand ??= new RelayCommand(param => ShowHideLands(), param => Can_ShowHideLands());
            }
        }

        public InMatchTrackerStateVM(
            IMapper mapper,
            CardThumbnailDownloader cardThumbnailDownloader,
            Dictionary<int, Card> dictAllCards
            )
        {
            this.mapper = mapper;
            this.cardThumbnailDownloader = cardThumbnailDownloader;
            this.dictAllCards = dictAllCards;
            MySideboard = new CardsListVM(DisplayType.CountOnly, CardsListOrder.ManaCost, this.mapper, cardThumbnailDownloader);

            OpponentCardsSeen = new CardsListVM(DisplayType.CountOnly, CardsListOrder.ManaCost, this.mapper, cardThumbnailDownloader);
            OpponentCardsSeen.CardsUpdated += OnOpponentCardsGotUpdated;

            OpponentCardsPrevGames = new CardsListVM(DisplayType.CountOnly, CardsListOrder.ManaCost, this.mapper, cardThumbnailDownloader);
            MyLibrary = new CardsListVM(DisplayType.Percent, CardsListOrder.ManaCost, this.mapper, cardThumbnailDownloader);
            MyLibraryWithoutLands = new CardsListVM(DisplayType.Percent, CardsListOrder.ManaCost, this.mapper, cardThumbnailDownloader);
        }

        internal void Init(CardsListOrder cardsListOrder, bool cardListCollapsed)
        {
            MyLibrary.CardsListOrder = cardsListOrder;
            MyLibraryWithoutLands.CardsListOrder = cardsListOrder;

            // Set the initial state of the compression
            SetCompressedCardList(cardListCollapsed);
        }

        private void OnOpponentCardsGotUpdated(object sender, EventArgs e)
        {
            OpponentCardsUpdated?.Invoke(this, null);
        }

        internal void Reset()
        {
            PriorityPlayer = 0;
            ActionLog.Clear();
            MyLibrary.ResetCards();
            MyLibraryWithoutLands.ResetCards();
            OpponentCardsSeen.ResetCards();
            OpponentCardsPrevGames.ResetCards();
            MySideboard.ResetCards();
            //FullDeck.ResetCards();

            TimerMe.Reset();
            TimerOpponent.Reset();

            GameEnded?.Invoke(this, null);

            // notify that everything changed
            OnPropertyChanged(string.Empty);
        }

        internal void SetInMatchStateBuffered(IInGameState state)
        {
            lock (LockCardsInMatchTracking)
            {
                //cardThumbnailDownloader.CheckAndDownloadThumbnails(state.OpponentCardsSeen.Select(i => i.GrpId).ToArray());

                MustReset |= state.IsReset;
                StateBuffered = state;
                UpdateCardsInMatchTrackingBuffered = true;
            }
        }

        internal void SetInMatchStateFromBuffered()
        {
            if (UpdateCardsInMatchTrackingBuffered == false)
                return;

            if (MustReset)
            {
                Reset();
                MustReset = false;
            }

            if (StateBuffered.MySeatId == 0)
                return;

            lock (LockCardsInMatchTracking)
            {
                OpponentScreenName = StateBuffered.OpponentScreenName;

                TurnNumber = StateBuffered.TurnNumber;

                OnThePlay = StateBuffered.OnThePlay == PlayerEnum.Unknown
                    ? ""
                    : TurnNumber < 2
                        ? StateBuffered.OnThePlay == PlayerEnum.Opponent
                            ? " (opponent goes first)"
                            : " (you go first)"
                        : StateBuffered.OnThePlay == PlayerEnum.Opponent
                            ? " (opponent went first)"
                            : " (you went first)";

                // Timers
                if (StateBuffered.IsSideboarding)
                {
                    TimerMe.Pause();
                    TimerOpponent.Pause();
                }
                if (PriorityPlayer != StateBuffered.PriorityPlayer)
                {
                    PriorityPlayer = StateBuffered.PriorityPlayer;

                    if (PriorityPlayer == StateBuffered.MySeatId)
                    {
                        TimerMe.Resume();
                        TimerOpponent.Pause();
                    }
                    else if (PriorityPlayer == StateBuffered.OpponentSeatId)
                    {
                        TimerMe.Pause();
                        TimerOpponent.Resume();
                    }
                }

                // Cards lists
                if (StateBuffered.MySeatId > 0)
                {
                    try
                    {
                        MyLibrary.ConvertCardList(StateBuffered.MyLibrary);
                        MyLibraryWithoutLands.ConvertCardList(StateBuffered.MyLibrary.Where(i => dictAllCards[i.GrpId].type.Contains("Land") == false).ToArray());

                        OpponentCardsSeen.ConvertCardList(StateBuffered.OpponentCardsSeen);
                        OpponentCardsPrevGames.ConvertCardList(StateBuffered.OpponentCardsPrevGames);

                        MySideboard.ConvertCardList(StateBuffered.MySideboard);

                        LibraryDrawManaBySource = CalcManaStats(StateBuffered.MyLibrary);

                        AddActionLogs(StateBuffered.GameEvents);
                    }
                    catch (KeyNotFoundException)
                    {
                        // Swallow this reported bug: "Got a crash after I conceded a game"
                        // I suppose the MySeatId got to 0 after the validation
                    }

                    OnPropertyChanged(string.Empty);
                }

                //if (isInitialized == false && stateBuffered.CardsByZone[stateBuffered.MySeatId][ZoneSimpleEnum.ZoneType_Library].Count > 0)
                //{
                //    FullDeck.ConvertCardList(stateBuffered.CardsByZone[stateBuffered.MySeatId][ZoneSimpleEnum.ZoneType_Library].ToDictionary(i => i.Key, i => i.Value.Count));
                //    OnPropertyChanged(nameof(FullDeck));
                //    isInitialized = true;
                //}

                UpdateCardsInMatchTrackingBuffered = false;
            }
        }

        private void AddActionLogs(IReadOnlyList<IGameEvent> gameEvents)
        {
            if (ActionLog.Count > gameEvents.Count)
                ActionLog.Clear();

            foreach (var gameEvent in gameEvents.Skip(ActionLog.Count))
            {
                ActionLog.Add(new ActionLogEntry(gameEvent));
            }
        }

        private Dictionary<string, ManaSource> CalcManaStats(IReadOnlyCollection<CardDrawInfo> library)
        {
            var manaInfo = new[] { "W", "U", "B", "R", "G", "C" }
                .Select(str =>
                {
                    var sourceAmount = library
                        .Where(c => dictAllCards[c.GrpId].ThisFaceProducesManaOfColor(str))
                        .Aggregate((amount: 0, chance: 0f), (x, info) => (x.amount + info.Amount, x.chance + info.DrawChance));

                    var includingMdfcLands = library
                        .Where(c => dictAllCards[c.GrpId].DoesProduceManaOfColor(str))
                        .Aggregate((amount: 0, chance: 0f), (x, info) => (x.amount + info.Amount, x.chance + info.DrawChance));

                    return new
                    {
                        Color = str,
                        Amount = sourceAmount.amount,
                        InclMdfcs = includingMdfcLands.amount,
                        Pct = sourceAmount.chance,
                        PctIncl = includingMdfcLands.chance,
                    };
                })
                .Where(i => i.InclMdfcs > 0);

            var manaBySource = manaInfo
                .ToDictionary(i => i.Color, i => new ManaSource
                {
                    Info = $"{i.Pct:p1} ({i.Amount} land{(i.Amount == 1 ? "" : "s")})",
                    InfoIncludingSpellLands = i.InclMdfcs - i.Amount == 0
                        ? ""
                        : $"{i.PctIncl:p1} including {i.InclMdfcs - i.Amount} spell land{(i.InclMdfcs - i.Amount == 1 ? "" : "s")}"
                });
            return manaBySource;
        }

        internal void SetCompressedCardList(bool compressed)
        {
            MyLibrary.ShowImage = !compressed;
            MyLibraryWithoutLands.ShowImage = !compressed;
            MySideboard.ShowImage = !compressed;
            OpponentCardsSeen.ShowImage = !compressed;
            OpponentCardsPrevGames.ShowImage = !compressed;
        }
    }
}
