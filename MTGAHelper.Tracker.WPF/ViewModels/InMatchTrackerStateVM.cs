using System.Collections.Generic;
using AutoMapper;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Tracker.WPF.Tools;
using MTGAHelper.Tracker.WPF.Business;
using System.Linq;
using System.Windows.Input;
using MTGAHelper.Entity;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
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

        readonly IMapper mapper;
        private readonly CardThumbnailDownloader cardThumbnailDownloader;
        private readonly Dictionary<int, Card> dictAllCards;

        public InMatchTrackerStateVM(IMapper mapper, CardThumbnailDownloader cardThumbnailDownloader, Dictionary<int, Card> dictAllCards)
        {
            this.mapper = mapper;
            this.cardThumbnailDownloader = cardThumbnailDownloader;
            this.dictAllCards = dictAllCards;
            MySideboard = new CardsListVM(DisplayType.CountOnly, CardsListOrder.ManaCost, this.mapper, cardThumbnailDownloader);
            OpponentCardsSeen = new CardsListVM(DisplayType.CountOnly, CardsListOrder.ManaCost, this.mapper, cardThumbnailDownloader);
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

        private Dictionary<string, string> _LibraryDrawManaBySource = new Dictionary<string, string>();
        public Dictionary<string, string> LibraryDrawManaBySource
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

        #endregion

        #endregion

        internal void Reset()
        {
            PriorityPlayer = 0;
            MyLibrary.ResetCards();
            MyLibraryWithoutLands.ResetCards();
            OpponentCardsSeen.ResetCards();
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

                        MySideboard.ConvertCardList(StateBuffered.MySideboard);

                        var totalCards = (float)StateBuffered.MyLibrary.Sum(i => i.Amount);
                        LibraryDrawManaBySource = new[] { "W", "U", "B", "R", "G", "C" }
                            .Select(i => {
                                var sourceAmount = StateBuffered.MyLibrary
                                    .Where(c =>
                                        dictAllCards[c.GrpId].type.Contains("Land") &&
                                        (i == "C" ? dictAllCards[c.GrpId].color_identity.Count == 0 : dictAllCards[c.GrpId].color_identity.Contains(i)))
                                    .Sum(i => i.Amount);

                                return new
                                {
                                    Color = i,
                                    Amount = sourceAmount,
                                    Pct = sourceAmount / totalCards
                                };
                            })
                            .Where(i => i.Pct != 0)
                            .ToDictionary(i => i.Color, i => $"{i.Pct:p1} ({i.Amount})");
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
        }
    }
}
