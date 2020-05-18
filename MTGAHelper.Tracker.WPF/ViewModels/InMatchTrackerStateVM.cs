using System.Collections.Generic;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Tracker.WPF.Tools;

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

        internal void Init(CardsListOrder cardsListOrder)
        {
            MyLibrary.CardsListOrder = cardsListOrder;
        }

        private readonly object LockCardsInMatchTracking = new object();

        private int PriorityPlayer;

        private bool MustReset;

        #region Bindings

        /// <summary>
        /// Current Deck
        /// </summary>
        public CardsListVM MyLibrary { get; } = new CardsListVM(DisplayType.Percent, CardsListOrder.ManaCost);

        /// <summary>
        /// Opponent Deck
        /// </summary>
        public CardsListVM OpponentCardsSeen { get; } = new CardsListVM(DisplayType.CountOnly, CardsListOrder.ManaCost);

        /// <summary>
        /// Sideboard
        /// </summary>
        public CardsListVM MySideboard { get; } = new CardsListVM(DisplayType.CountOnly, CardsListOrder.ManaCost);

        //public CardsListVM FullDeck { get; set; } = new CardsListVM(false);

        /// <summary>
        /// String version of lands remaining over starting lands
        /// </summary>
        public string LibraryLandCurrentAndTotal => $"{MyLibrary.LandCount} / {MyLibrary.TotalLandsInitial}";

        /// <summary>
        /// Percent chance to draw a land
        /// </summary>
        public float LibraryDrawLandPct => MyLibrary.DrawLandPct;

        public bool SplitLands
        {
            get => _SplitLands;
            set => SetField(ref _SplitLands, value, nameof(SplitLands));
        }

        private bool _SplitLands;

        public int LibraryCardsCount => MyLibrary.CardCount;

        public int SideboardCardsCount => MySideboard.CardCount;

        public int OpponentCardsSeenCount => OpponentCardsSeen.CardCount;

        public PlayerTimerVM TimerMe { get; set; } = new PlayerTimerVM();

        public PlayerTimerVM TimerOpponent { get; set; } = new PlayerTimerVM();

        #endregion

        internal void Reset()
        {
            PriorityPlayer = 0;
            MyLibrary.ResetCards();
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

                        OpponentCardsSeen.ConvertCardList(StateBuffered.OpponentCardsSeen);

                        MySideboard.ConvertCardList(StateBuffered.MySideboard);
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
    }
}
