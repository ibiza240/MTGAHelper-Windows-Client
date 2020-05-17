using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Tracker.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class DraftInfoBuffered
    {
        public DraftPickProgress DraftProgress { get; set; } = new DraftPickProgress();

        public ICollection<CardDraftPickWpf> CardsDraftBuffered { get; set; } = new CardDraftPickWpf[0];
    }

    public class DraftingVM : ObservableObject
    {
        #region Dependancy Injection Initializer

        /// <summary>
        /// Initialize the class using dependency injection
        /// </summary>
        /// <param name="all"></param>
        internal void Init(ICollection<Card> all)
        {
            AllCards = all;
        }

        #endregion

        #region Constants

        /// <summary>
        /// Number of drafters (bots) in the draft pod
        /// </summary>
        private const int POD_SIZE = 8;

        #endregion

        #region Public Properties

        /// <summary>
        /// Whether to show the card list that did not wheel
        /// </summary>
        public bool ShowCardListThatDidNotWheel
        {
            get => _ShowCardListThatDidNotWheel;
            set => SetField(ref _ShowCardListThatDidNotWheel, value, nameof(ShowCardListThatDidNotWheel));
        }

        /// <summary>
        /// The Pack and Pick message string
        /// </summary>
        public string PackPickMessage => DraftInfoBuffered.DraftProgress.DraftId == default ? "" : $"Pack {DraftInfoBuffered.DraftProgress.PackNumber + 1} Pick {DraftInfoBuffered.DraftProgress.PickNumber + 1}";

        /// <summary>
        /// Whether a message is visible when no cards wheeled
        /// </summary>
        public bool IsVisibleWhenCardsDidNotWheel => CardsThatDidNotWheel.Count > 0;

        /// <summary>
        /// Card wheeling message string
        /// </summary>
        public string CardsWheelingMessage => CardsThatDidNotWheel.Count > 0 ? $"{CardsThatDidNotWheel.Count} cards didn't wheel from this pack" :
            (/*CurrentDraftPickProgress.PickNumber >= POD_SIZE ? "This pack won't wheel" :*/ $"{(NumCardsWheeling == 0 ? "No" : NumCardsWheeling.ToString())} card{(NumCardsWheeling == 1 ? " is" : "s are")} wheeling in this pack");

        /// <summary>
        /// List of cards that did not wheel
        /// </summary>
        public CardsListVM CardsThatDidNotWheelVM { get; set; } = new CardsListVM(DisplayType.None, CardsListOrder.ManaCost);

        public DraftPickProgress CurrentDraftPickProgress => DraftInfoBuffered.DraftProgress;

        public bool ShowGlobalMTGAHelperSays => /*updateCardsDraftBuffered == false && */CardsDraft.Count > 0 && CardsDraft.Count < 42;

        public ICollection<CardDraftPickVM> CardsDraft { get; set; } = new CardDraftPickVM[0];

        public ICollection<int> CardsThatDidNotWheel { get; set; } = new int[0];

        public string CardChosenThatDidNotWheel { get; private set; }

        public double RareDraftOpacity => CardsDraft.Any(x => x.RareDraftPickEnum == RaredraftPickReasonEnum.None && Math.Abs(x.RatingValue) > float.Epsilon) ? 1.0d : 0.5d;

        #endregion

        #region Private Backing Fields

        /// <summary>
        /// Whether to show the card list that did not wheel
        /// </summary>
        private bool _ShowCardListThatDidNotWheel;

        #endregion

        #region Private Fields

        private int NumCardsWheeling => Math.Max(0, CardsDraft.Count - POD_SIZE);

        private ICollection<Card> AllCards = new Card[0];

        private bool UpdateCardsDraftBuffered;

        private readonly object LockCardsDraft = new object();

        private readonly DraftInfoBuffered DraftInfoBuffered = new DraftInfoBuffered();

        private List<DraftPickProgress> DraftPicksHistory = new List<DraftPickProgress>();

        #endregion

        #region Internal Methods

        internal void SetCardsDraftBuffered(DraftPickProgress draftProgress, ICollection<CardDraftPickWpf> ratingsInfo)
        {
            lock (LockCardsDraft)
            {
                // Manage the packs history, this is all for the "previously seen cards that didn't wheel" feature
                // (this if condition is only there to manage the case where the ratings source was changed, thus triggering an update here
                //  and we don't want that to affect the history)
                if (draftProgress.PackNumber != CurrentDraftPickProgress.PackNumber || draftProgress.PickNumber != CurrentDraftPickProgress.PickNumber)
                {
                    CardsThatDidNotWheel = new int[0];

                    // Reset the progress history if it is a new draft
                    if (draftProgress.DraftId != default && DraftPicksHistory.Count > 0 && draftProgress.DraftId != DraftPicksHistory[0].DraftId)
                    {
                        DraftPicksHistory = new List<DraftPickProgress> { draftProgress };
                    }

                    // Try to find the original pack that wheeled to extract the cards that didn't wheel
                    var originalPack = DraftPicksHistory.FirstOrDefault(i => i.PackNumber == draftProgress.PackNumber && i.PickNumber == draftProgress.PickNumber - POD_SIZE);
                   
                    if (originalPack != null)
                    {
                        CardsThatDidNotWheel = originalPack.DraftPack.Where(i => draftProgress.DraftPack.Contains(i) == false).ToArray();
                       
                        var packFollowing = DraftPicksHistory.FirstOrDefault(i => i.PackNumber == originalPack.PackNumber && i.PickNumber == originalPack.PickNumber + 1);
                        
                        if (packFollowing != null)
                            CardChosenThatDidNotWheel = Mapper.Map<Card>(packFollowing.PickedCards.Last()).name;
                    }

                    DraftPicksHistory.Add(draftProgress);
                }

                DraftInfoBuffered.DraftProgress = draftProgress;

                DraftInfoBuffered.CardsDraftBuffered = ratingsInfo;

                UpdateCardsDraftBuffered = true;
            }
        }

        internal void ToggleShowHideCardListPopupThatDidNotWheel()
        {
            ShowCardListThatDidNotWheel = !ShowCardListThatDidNotWheel;
        }

        internal void SetCardsDraftFromBuffered()
        {
            if (UpdateCardsDraftBuffered == false)
                return;

            lock (LockCardsDraft)
            {
                CardsDraft = Mapper.Map<ICollection<CardDraftPickVM>>(DraftInfoBuffered.CardsDraftBuffered);

                UpdateCardsDraftBuffered = false;
                RaisePropertyChangedEvent(nameof(CardsDraft));
                RaisePropertyChangedEvent(nameof(CardsWheelingMessage));
                RaisePropertyChangedEvent(nameof(PackPickMessage));
                RaisePropertyChangedEvent(nameof(ShowGlobalMTGAHelperSays));
                RaisePropertyChangedEvent(nameof(IsVisibleWhenCardsDidNotWheel));

                // Setup the card list popup content
                var cardsWheeled = AllCards
                    .Where(i => CardsThatDidNotWheel.Contains(i.grpId))
                    .Select(Mapper.Map<CardWpf>)
                    .ToArray();

                CardsThatDidNotWheelVM.SetCards(CardChosenThatDidNotWheel, cardsWheeled);

                RaisePropertyChangedEvent(nameof(CardsThatDidNotWheelVM));
            }
        }

        #endregion

        //int nbCardsWheeling => Math.Max(0, CardsDraftByTier.Values.Sum(i => i.Count) - POD_SIZE);

        //public Dictionary<float, ICollection<CardDraftPickVM>> CardsDraftByTier { get; set; } = new Dictionary<float, ICollection<CardDraftPickVM>>();
    }
}
