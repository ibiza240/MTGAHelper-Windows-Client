using System.Collections.Generic;
using AutoMapper;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Tracker.WPF.Tools;
using MTGAHelper.Tracker.WPF.Business;
using System.Linq;
using System.Windows.Input;
using MTGAHelper.Entity;
using MTGAHelper.Entity.MtgaOutputLog;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class ManaSource
    {
        public string Info { get; set; }
        public string InfoIncludingSpellLands { get; set; }
    }

    public class InMatchTrackerStateVM : BasicModel
    {
        //enum CardListTypeEnum
        //{
        //    Unknown,
        //    MyLibrary,
        //    OpponentCards,
        //    MyFullDeck,
        //}

        private IInGameState StateBuffered;

        private bool UpdateCardsInMatchTrackingBuffered;

        private readonly IMapper mapper;
        private readonly CardThumbnailDownloader cardThumbnailDownloader;
        private readonly Dictionary<int, Card> dictAllCards;

        public InMatchTrackerStateVM(IMapper mapper, CardThumbnailDownloader cardThumbnailDownloader, Dictionary<int, Card> dictAllCards)
        {
            this.mapper = mapper;
            this.cardThumbnailDownloader = cardThumbnailDownloader;
            this.dictAllCards = dictAllCards;
            MySideboard = new CardsListVM(DisplayType.CountOnly, CardsListOrder.ManaCost, this.mapper, cardThumbnailDownloader);
            OpponentCardsSeen = new CardsListVM(DisplayType.CountOnly, CardsListOrder.ManaCost, this.mapper, cardThumbnailDownloader);
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

        private readonly object LockCardsInMatchTracking = new object();

        private int PriorityPlayer;

        private bool MustReset;

        #region Bindings

        public int TurnNumber { get; private set; }

        public string OnThePlay { get; private set; }

        /// <summary>
        /// Current Deck
        /// </summary>
        public CardsListVM MyLibrary { get; }

        public CardsListVM MyLibraryWithoutLands { get; }

        /// <summary>
        /// Opponent Deck
        /// </summary>
        public CardsListVM OpponentCardsSeen { get; }

        /// <summary>
        /// Opponent cards seen in previous games of BO3
        /// </summary>
        public CardsListVM OpponentCardsPrevGames { get; }

        /// <summary>
        /// Sideboard
        /// </summary>
        public CardsListVM MySideboard { get; }

        //public CardsListVM FullDeck { get; set; } = new CardsListVM(false);

        /// <summary>
        /// String version of lands remaining over starting lands
        /// </summary>
        public string LibraryLandCurrentAndTotal => $"{MyLibrary.LandCount} / {MyLibrary.TotalLandsInitial}";

        /// <summary>
        /// Percent chance to draw a land
        /// </summary>
        public float LibraryDrawLandPct => MyLibrary.DrawLandPct;

        //private bool _SplitLands;
        //public bool SplitLands
        //{
        //    get => _SplitLands;
        //    set => SetField(ref _SplitLands, value, nameof(SplitLands));
        //}

        private Dictionary<string, ManaSource> _LibraryDrawManaBySource = new Dictionary<string, ManaSource>();

        public Dictionary<string, ManaSource> LibraryDrawManaBySource
        {
            get => _LibraryDrawManaBySource;
            set => SetField(ref _LibraryDrawManaBySource, value, nameof(LibraryDrawManaBySource));
        }

        /// <summary>
        /// Whether to show the lands in the list
        /// </summary>
        private bool _ShowCardLands = true;

        public bool ShowLands
        {
            get => _ShowCardLands;
            set => SetField(ref _ShowCardLands, value, nameof(ShowLands));
        }

        public int LibraryCardsCount => MyLibrary.CardCount;

        public int SideboardCardsCount => MySideboard.CardCount;

        public int OpponentCardsSeenCount => OpponentCardsSeen.CardCount;
        public int OpponentCardsPrevGamesCount => OpponentCardsPrevGames.CardCount;

        public string OpponentScreenName { get; set; }

        public PlayerTimerVM TimerMe { get; set; } = new PlayerTimerVM();

        public PlayerTimerVM TimerOpponent { get; set; } = new PlayerTimerVM();

        #region Show Hide Lands Command

        public ICommand ShowHideLandsCommand
        {
            get
            {
                return _ShowHideLandsCommand ??= new RelayCommand(param => ShowHideLands(), param => Can_ShowHideLands());
            }
        }

        private ICommand _ShowHideLandsCommand;

        private static bool Can_ShowHideLands() => true;

        private void ShowHideLands() => ShowLands = !ShowLands;

        #endregion Show Hide Lands Command

        #endregion Bindings

        internal void Reset()
        {
            PriorityPlayer = 0;
            MyLibrary.ResetCards();
            MyLibraryWithoutLands.ResetCards();
            OpponentCardsSeen.ResetCards();
            OpponentCardsPrevGames.ResetCards();
            MySideboard.ResetCards();
            //FullDeck.ResetCards();

            TimerMe.Reset();
            TimerOpponent.Reset();

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

                        var totalCards = (float)StateBuffered.MyLibrary.Sum(i => i.Amount);

                        var manaInfo = new[] { "W", "U", "B", "R", "G", "C" }
                            .Select(str =>
                            {
                                var sourceAmount = StateBuffered.MyLibrary
                                    .Where(c => dictAllCards[c.GrpId].ThisFaceProducesManaOfColor(str))
                                    .Sum(c => c.Amount);

                                var includingMdfcLands = StateBuffered.MyLibrary
                                    .Where(c => dictAllCards[c.GrpId].DoesProduceManaOfColor(str))
                                    .Sum(c => c.Amount);

                                return new
                                {
                                    Color = str,
                                    Amount = sourceAmount,
                                    InclMdfcs = includingMdfcLands,
                                    Pct = sourceAmount / totalCards,
                                    PctIncl = includingMdfcLands / totalCards,
                                };
                            })
                            .Where(i => i.InclMdfcs > 0);

                        LibraryDrawManaBySource = manaInfo
                            .ToDictionary(i => i.Color, i => new ManaSource
                            {
                                Info = $"{i.Pct:p1} ({i.Amount} land{(i.Amount == 1 ? "" : "s")})",
                                InfoIncludingSpellLands = i.InclMdfcs - i.Amount == 0 ? "" : $"{i.PctIncl:p1} including {i.InclMdfcs - i.Amount} spell land{(i.InclMdfcs - i.Amount == 1 ? "" : "s")}"
                            });
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

        /// <summary>
        /// Set the compression state of the match state card lists
        /// </summary>
        /// <param name="compressed"></param>
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
