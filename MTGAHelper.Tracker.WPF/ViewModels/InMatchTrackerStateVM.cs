using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using AutoMapper;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;
using MTGAHelper.Tracker.WPF.Models;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class Stats : ObservableObject
    {
        public Dictionary<int, int> AmountOriginalByGrpId { get; set; } = new Dictionary<int, int>();
        public ObservableProperty<float> DrawLandPct { get; set; } = new ObservableProperty<float>(0f);
        public ObservableProperty<int> TotalLandsInitial { get; set; } = new ObservableProperty<int>(0);

        internal void Refresh(ICollection<LibraryCardWithAmountVM> myDeck)
        {
            if (myDeck == null)
                return;

            var totalCards = myDeck.Sum(i => i.Amount);
            if (totalCards == 0)
                DrawLandPct.Value = 0;
            else
                DrawLandPct.Value = (float)myDeck.Where(i => i.Type.Contains("Land")).Sum(i => i.Amount) / totalCards;

            if (AmountOriginalByGrpId.Any() == false)
            {
                // Store the original amounts on first time
                AmountOriginalByGrpId = myDeck.ToDictionary(i => i.ArenaId, i => i.Amount);
            }

        }

        internal void Reset()
        {
            DrawLandPct.Value = 0;
            AmountOriginalByGrpId = new Dictionary<int, int>();
        }
    }

    public class InMatchTrackerStateVM : ObservableObject
    {
        //enum CardListTypeEnum
        //{
        //    Unknown,
        //    MyLibrary,
        //    OpponentCards,
        //    MyFullDeck,
        //}

        InGameTrackerState stateBuffered;

        bool updateCardsInMatchTrackingBuffered { get; set; } = false;

        internal void RefreshSplitLands()
        {
        }

        object lockCardsInMatchTracking = new object();
        int priorityPlayer;
        bool mustReset;

        ////Dictionary<CardListTypeEnum, List<LibraryCardWithAmountVM>> CardLists = new Dictionary<CardListTypeEnum, List<LibraryCardWithAmountVM>>
        ////{
        ////    { CardListTypeEnum.MyLibrary, new List<LibraryCardWithAmountVM>() },
        ////    { CardListTypeEnum.OpponentCards, new List<LibraryCardWithAmountVM>() },
        ////    { CardListTypeEnum.MyFullDeck, new List<LibraryCardWithAmountVM>() },
        ////};
        //List<LibraryCardWithAmountVM> myLibrary = new List<LibraryCardWithAmountVM>();
        //List<LibraryCardWithAmountVM> opponentCardsSeen = new List<LibraryCardWithAmountVM>();
        //List<LibraryCardWithAmountVM> fullDeck = new List<LibraryCardWithAmountVM>();

        #region Bindings
        //public List<LibraryCardWithAmountVM> MyDeck => myLibrary;
        //public List<LibraryCardWithAmountVM> OpponentCardsSeen => opponentCardsSeen;
        //public List<LibraryCardWithAmountVM> FullDeck => fullDeck;
        public CardsListVM MyLibrary { get; set; } = new CardsListVM(true, false);
        public CardsListVM OpponentCardsSeen { get; set; } = new CardsListVM(false, true);
        public CardsListVM MySideboard { get; set; } = new CardsListVM(false, true);
        //public CardsListVM FullDeck { get; set; } = new CardsListVM(false);

        public string LibraryLandCurrentAndTotal => $"{LibraryLandsCount} / {Stats.TotalLandsInitial.Value}";

        public ObservableProperty<bool> SplitLands { get; set; } = new ObservableProperty<bool>(true);
        public bool ShowWindowOpponentCardsSeen { get; set; }

        public int LibraryCardsCount => MyLibrary.Cards.Sum(i => i.Amount);
        public int LibraryLandsCount => MyLibrary.Cards.Where(i => i.Type.Contains("Land")).Sum(i => i.Amount);

        public int SideboardCardsCount => MySideboard.Cards.Sum(i => i.Amount);

        public Stats Stats { get; set; } = new Stats();
        public PlayerTimerVM TimerMe { get; set; } = new PlayerTimerVM();
        public PlayerTimerVM TimerOpponent { get; set; } = new PlayerTimerVM();
        #endregion

        internal void Reset()
        {
            priorityPlayer = 0;
            MyLibrary.ResetCards(Stats);
            OpponentCardsSeen.ResetCards();
            MySideboard.ResetCards();
            //FullDeck.ResetCards();

            Stats.Reset();
            TimerMe.Reset();
            TimerOpponent.Reset();

            RaisePropertyChangedEvent(nameof(MyLibrary));
            RaisePropertyChangedEvent(nameof(OpponentCardsSeen));
            RaisePropertyChangedEvent(nameof(MySideboard));
            //RaisePropertyChangedEvent(nameof(FullDeck));
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
                    var cardsLibrary = stateBuffered.CardsByZone[stateBuffered.MySeatId][ZoneSimpleEnum.ZoneType_Library]
                        .ToDictionary(i => i.Key, i => i.Value.Count);

                    MyLibrary.ConvertCardList(cardsLibrary);

                    var opponentCards = stateBuffered.OpponentCardsSeen
                        .GroupBy(i => i.GrpId)
                        .ToDictionary(i => i.Key, i => i.Count());

                    OpponentCardsSeen.ConvertCardList(opponentCards);

                    MySideboard.ConvertCardList(stateBuffered.CardsByZone[stateBuffered.MySeatId][ZoneSimpleEnum.ZoneType_Sideboard]
                        .ToDictionary(i => i.Key, i => i.Value.Count));

                    RaisePropertyChangedEvent(nameof(MyLibrary));
                    RaisePropertyChangedEvent(nameof(MySideboard));
                    //RaisePropertyChangedEvent(nameof(FullDeck));
                    RaisePropertyChangedEvent(nameof(LibraryCardsCount));
                    RaisePropertyChangedEvent(nameof(LibraryLandsCount));
                    RaisePropertyChangedEvent(nameof(OpponentCardsSeen));
                    RaisePropertyChangedEvent(nameof(LibraryLandCurrentAndTotal));
                }

                // Stats
                Stats.Refresh(MyLibrary.Cards);

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
