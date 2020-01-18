using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class InMatchTrackerStateVM : ObservableObject
    {
        //enum CardListTypeEnum
        //{
        //    Unknown,
        //    MyLibrary,
        //    OpponentCards,
        //    MyFullDeck,
        //}

        IInGameState stateBuffered;

        bool updateCardsInMatchTrackingBuffered;

        internal void Init(CardsListOrder cardsListOrder)
        {
            MyLibrary.CardsListOrder = cardsListOrder;
        }

        readonly object lockCardsInMatchTracking = new object();
        int priorityPlayer;
        bool mustReset;

        #region Bindings
        public CardsListVM MyLibrary { get; } = new CardsListVM(true, false, CardsListOrder.Cmc);
        public CardsListVM OpponentCardsSeen { get; } = new CardsListVM(false, true, CardsListOrder.Cmc);
        public CardsListVM MySideboard { get; } = new CardsListVM(false, true, CardsListOrder.Cmc);
        //public CardsListVM FullDeck { get; set; } = new CardsListVM(false);

        public string LibraryLandCurrentAndTotal => $"{MyLibrary.LandCount} / {MyLibrary.TotalLandsInitial}";
        public float LibraryDrawLandPct => MyLibrary.DrawLandPct;

        public ObservableProperty<bool> SplitLands { get; set; } = new ObservableProperty<bool>(true);
        public bool ShowWindowOpponentCardsSeen { get; set; }

        public int LibraryCardsCount => MyLibrary.CardCount;
        public int LibraryLandsCount => MyLibrary.LandCount;

        public int SideboardCardsCount => MySideboard.CardCount;

        public PlayerTimerVM TimerMe { get; set; } = new PlayerTimerVM();
        public PlayerTimerVM TimerOpponent { get; set; } = new PlayerTimerVM();
        #endregion

        internal void Reset()
        {
            priorityPlayer = 0;
            MyLibrary.ResetCards();
            OpponentCardsSeen.ResetCards();
            MySideboard.ResetCards();
            //FullDeck.ResetCards();

            TimerMe.Reset();
            TimerOpponent.Reset();

            // notify that everything changed
            RaisePropertyChangedEvent(string.Empty);
        }

        internal void SetInMatchStateBuffered(InGameTrackerState state)
        {
            lock (lockCardsInMatchTracking)
            {
                mustReset |= state.IsReset;
                stateBuffered = state;
                updateCardsInMatchTrackingBuffered = true;
            }
        }

        internal void SetInMatchStateFromBuffered()
        {
            if (updateCardsInMatchTrackingBuffered == false)
                return;

            if (mustReset)
            {
                Reset();
                mustReset = false;
            }

            if (stateBuffered.MySeatId == 0)
                return;

            lock (lockCardsInMatchTracking)
            {
                // Timers
                if (stateBuffered.IsSideboarding)
                {
                    TimerMe.Pause();
                    TimerOpponent.Pause();
                }
                if (priorityPlayer != stateBuffered.PriorityPlayer)
                {
                    priorityPlayer = stateBuffered.PriorityPlayer;

                    if (priorityPlayer == stateBuffered.MySeatId)
                    {
                        TimerMe.Unpause();
                        TimerOpponent.Pause();
                    }
                    else if (priorityPlayer == stateBuffered.OpponentSeatId)
                    {
                        TimerMe.Pause();
                        TimerOpponent.Unpause();
                    }
                }

                // Cards lists
                if (stateBuffered.MySeatId > 0)
                {
                    try
                    {
                        MyLibrary.ConvertCardList(stateBuffered.CardsInLibrary);

                        OpponentCardsSeen.ConvertCardList(stateBuffered.OpponentCardsSeen);

                        MySideboard.ConvertCardList(stateBuffered.CardsInSideboard);
                    }
                    catch (KeyNotFoundException)
                    {
                        // Swallow this reported bug: "Got a crash after I conceded a game"
                        // I suppose the MySeatId got to 0 after the validation
                    }

                    RaisePropertyChangedEvent(string.Empty);
                }

                //if (isInitialized == false && stateBuffered.CardsByZone[stateBuffered.MySeatId][ZoneSimpleEnum.ZoneType_Library].Count > 0)
                //{
                //    FullDeck.ConvertCardList(stateBuffered.CardsByZone[stateBuffered.MySeatId][ZoneSimpleEnum.ZoneType_Library].ToDictionary(i => i.Key, i => i.Value.Count));
                //    RaisePropertyChangedEvent(nameof(FullDeck));
                //    isInitialized = true;
                //}

                updateCardsInMatchTrackingBuffered = false;
            }
        }
    }
}
