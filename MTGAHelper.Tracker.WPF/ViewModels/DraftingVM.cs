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
        const int POD_SIZE = 8;

        ICollection<Card> allCards = new Card[0];

        bool updateCardsDraftBuffered;
        readonly object lockCardsDraft = new object();
        readonly DraftInfoBuffered draftInfoBuffered = new DraftInfoBuffered();
        List<DraftPickProgress> draftPicksHistory = new List<DraftPickProgress>();

        //int nbCardsWheeling => Math.Max(0, CardsDraftByTier.Values.Sum(i => i.Count) - POD_SIZE);
        int nbCardsWheeling => Math.Max(0, CardsDraft.Count - POD_SIZE);

        //public Dictionary<float, ICollection<CardDraftPickVM>> CardsDraftByTier { get; set; } = new Dictionary<float, ICollection<CardDraftPickVM>>();
        public ICollection<CardDraftPickVM> CardsDraft { get; set; } = new CardDraftPickVM[0];
        public string PackPickMessage => draftInfoBuffered.DraftProgress.DraftId == default(string) ? "" : $"Pack {draftInfoBuffered.DraftProgress.PackNumber + 1} pick {draftInfoBuffered.DraftProgress.PickNumber + 1}";

        public ICollection<int> CardsThatDidntWheel { get; set; } = new int[0];
        public string CardChosenThatDidntWheel { get; private set; }

        internal void Init(ICollection<Card> allCards)
        {
            this.allCards = allCards;
        }

        //public string CardsWheelingMessage => nbCardsWheeling == 0 ?
        //    (CurrentDraftPickProgress.PickNumber < POD_SIZE - 1 ? $"{CardsWheeled.Count} cards wheeled from this pack" : "This pack won't wheel") :
        //    $"{nbCardsWheeling} cards {(nbCardsWheeling == 1 ? "is" : "are")} wheeling in this pack";
        public string CardsWheelingMessage => CardsThatDidntWheel.Count > 0 ? $"{CardsThatDidntWheel.Count} cards didn't wheel from this pack" :
            (/*CurrentDraftPickProgress.PickNumber >= POD_SIZE ? "This pack won't wheel" :*/ $"{(nbCardsWheeling == 0 ? "No" : nbCardsWheeling.ToString())} card{(nbCardsWheeling == 1 ? " is" : "s are")} wheeling in this pack");
        public bool IsVisibleWhenCardsDidntWheel => CardsThatDidntWheel.Count > 0;
        public bool IsHiddenWhenCardsDidntWheel => !IsVisibleWhenCardsDidntWheel;

        public double RaredraftOpacity => CardsDraft.Any(x => x.RareDraftPickEnum == Entity.RaredraftPickReasonEnum.None && x.RatingFloat != 0f) ? 1.0d : 0.5d;
        public bool ShowGlobalMTGAHelperSays => /*updateCardsDraftBuffered == false && */CardsDraft.Count > 0 && CardsDraft.Count < 42;

        public ObservableProperty<bool> ShowCardListThatDidntWheel { get; set; } = new ObservableProperty<bool>(false);

        public DraftPickProgress CurrentDraftPickProgress => draftInfoBuffered.DraftProgress;
        public CardsListVM CardsThatDidntWheelVM { get; set; } = new CardsListVM(false, false, CardsListOrder.Cmc);

        internal void SetCardsDraftBuffered(DraftPickProgress draftProgress, ICollection<CardDraftPickWpf> ratingsInfo)
        {
            lock (lockCardsDraft)
            {
                // Manage the packs history, this is all for the "previously seen cards that didn't wheel" feature
                // (this if condition is only there to manage the case where the ratings source was changed, thus triggering an update here
                //  and we don't want that to affect the history)
                if (draftProgress.PackNumber != CurrentDraftPickProgress.PackNumber || draftProgress.PickNumber != CurrentDraftPickProgress.PickNumber)
                {
                    CardsThatDidntWheel = new int[0];

                    // Reset the progress history if it is a new draft
                    if (draftProgress.DraftId != default(string) && draftPicksHistory.Count > 0 && draftProgress.DraftId != draftPicksHistory[0].DraftId)
                    {
                        draftPicksHistory = new List<DraftPickProgress> { draftProgress };
                    }

                    // Try to find the original pack that wheeled to extract the cards that didn't wheel
                    var originalPack = draftPicksHistory.FirstOrDefault(i => i.PackNumber == draftProgress.PackNumber && i.PickNumber == draftProgress.PickNumber - POD_SIZE);
                    if (originalPack != null)
                    {
                        CardsThatDidntWheel = originalPack.DraftPack.Where(i => draftProgress.DraftPack.Contains(i) == false).ToArray();
                        var packFollowing = draftPicksHistory.FirstOrDefault(i => i.PackNumber == originalPack.PackNumber && i.PickNumber == originalPack.PickNumber + 1);
                        if (packFollowing != null)
                            CardChosenThatDidntWheel = Mapper.Map<Card>(packFollowing.PickedCards.Last()).name;
                    }

                    draftPicksHistory.Add(draftProgress);
                }

                draftInfoBuffered.DraftProgress = draftProgress;
                draftInfoBuffered.CardsDraftBuffered = ratingsInfo;

                updateCardsDraftBuffered = true;
            }
        }

        internal void ToggleShowHideCardListPopupThatDidntWheel()
        {
            ShowCardListThatDidntWheel.Value = !ShowCardListThatDidntWheel.Value;
        }

        internal void SetCardsDraftFromBuffered()
        {
            if (updateCardsDraftBuffered == false)
                return;

            //if (updateCardsDraftBuffered)
            //{
            var calculator = new BorderGradientCalculator();
            lock (lockCardsDraft)
            {
                var cardsVM = Mapper.Map<ICollection<CardDraftPickVM>>(draftInfoBuffered.CardsDraftBuffered);
                foreach (var c in cardsVM)
                    c.BorderGradient = calculator.CalculateBorderGradient(c);

                //CardsDraftByTier = cardsVM
                //    .GroupBy(i => i.RatingFloat)
                //    .ToDictionary(i => i.Key, i => (ICollection<CardDraftPickVM>)i.ToArray());
                CardsDraft = cardsVM;

                updateCardsDraftBuffered = false;
                RaisePropertyChangedEvent(nameof(CardsDraft));
                RaisePropertyChangedEvent(nameof(CardsWheelingMessage));
                RaisePropertyChangedEvent(nameof(PackPickMessage));
                RaisePropertyChangedEvent(nameof(ShowGlobalMTGAHelperSays));
                RaisePropertyChangedEvent(nameof(IsVisibleWhenCardsDidntWheel));
                RaisePropertyChangedEvent(nameof(IsHiddenWhenCardsDidntWheel));

                // Setup the cardlist popup content
                var cardsWheeled = allCards
                    .Where(i => CardsThatDidntWheel.Contains(i.grpId))
                    .Select(i => Mapper.Map<CardWpf>(i))
                    .ToArray();
                CardsThatDidntWheelVM.SetCards(CardChosenThatDidntWheel, cardsWheeled);
                RaisePropertyChangedEvent(nameof(CardsThatDidntWheelVM));
            }
            //}
        }
    }
}
