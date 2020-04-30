using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class Stats
    {
        private bool HasCapturedTotals;
        public int CardsLeftInDeck { get; private set; }
        public int LandsLeftInDeck { get; private set; }
        public int TotalCardsInitial { get; set; }
        public int TotalLandsInitial { get; private set; }
        public float DrawLandPct { get; private set; }

        internal void Refresh(ICollection<LibraryCardWithAmountVM> deck)
        {
            if (deck == null)
                return;

            CardsLeftInDeck = deck.Sum(i => i.Amount);
            LandsLeftInDeck = deck.Where(i => i.Type.Contains("Land")).Sum(i => i.Amount);
            DrawLandPct = deck.Where(c => c.Type.Contains("Land")).Sum(c => c.DrawPercent);

            if (HasCapturedTotals)
                return;

            TotalLandsInitial = LandsLeftInDeck;
            TotalCardsInitial = CardsLeftInDeck;
            HasCapturedTotals = true;
        }

        internal void Reset(ICollection<LibraryCardWithAmountVM> deck)
        {
            HasCapturedTotals = false;
            Refresh(deck);
        }

        internal void Reset()
        {
            HasCapturedTotals = false;

            CardsLeftInDeck = 0;
            LandsLeftInDeck = 0;
            DrawLandPct = 0;
            TotalLandsInitial = 0;
            TotalCardsInitial = 0;
        }
    }
}