using AutoMapper;
using MTGAHelper.Tracker.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class DraftingVM : ObservableObject
    {
        internal bool updateCardsDraftBuffered { get; private set; } = true;
        object lockCardsDraft = new object();

        #region Bindings
        public Dictionary<float, ICollection<CardDraftPickVM>> CardsDraftByTier { get; set; } = new Dictionary<float, ICollection<CardDraftPickVM>>();
        public ICollection<CardDraftPickVM> CardsDraftBuffered { get; set; } = new CardDraftPickVM[0];
        public int NbCardsWheeling => Math.Max(0, CardsDraftByTier.Values.Sum(i => i.Count) - 8);
        public string CardsWheelingMessage => $"{NbCardsWheeling} cards {(NbCardsWheeling == 1 ? "is" : "are")} wheeling in this pack";
        public double RaredraftOpacity => CardsDraftByTier.Any(i => i.Value.Any(x => x.RareDraftPickEnum == Entity.RaredraftPickReasonEnum.None && x.RatingFloat != 0f)) ? 1.0d : 0.5d;
        public bool ShowGlobalMTGAHelperSays => updateCardsDraftBuffered == false && CardsDraftByTier.Sum(i => i.Value.Count) < 45;
        #endregion

        internal void SetCardsDraftBuffered(ICollection<CardDraftPick> cards)
        {
            CardsDraftBuffered = Mapper.Map<ICollection<CardDraftPickVM>>(cards);
            updateCardsDraftBuffered = true;
        }

        internal void SetCardsDraftFromBuffered()
        {
            //if (updateCardsDraftBuffered)
            //{
            lock (lockCardsDraft)
            {
                foreach (var c in CardsDraftBuffered)
                    c.CardVM.SetColorGradiant();

                CardsDraftByTier = CardsDraftBuffered
                    .GroupBy(i => i.RatingFloat)
                    .ToDictionary(i => i.Key, i => (ICollection<CardDraftPickVM>)i.ToArray());

                updateCardsDraftBuffered = false;
                RaisePropertyChangedEvent(nameof(CardsDraftByTier));
                RaisePropertyChangedEvent(nameof(CardsWheelingMessage));
                RaisePropertyChangedEvent(nameof(ShowGlobalMTGAHelperSays));
            }
            //}
        }
    }
}
