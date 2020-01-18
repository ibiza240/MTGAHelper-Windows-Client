using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class Stats
    {
        bool hasCapturedTotals;
        public int CardsLeftInDeck { get; private set; }
        public int LandsLeftInDeck { get; private set; }
        public int TotalCardsInitial { get; private set; }
        public int TotalLandsInitial { get; private set; }
        public float DrawLandPct { get; private set; }

        internal void Refresh(ICollection<LibraryCardWithAmountVM> deck)
        {
            if (deck == null)
                return;

            CardsLeftInDeck = deck.Sum(i => i.Amount);
            LandsLeftInDeck = deck.Where(i => i.Type.Contains("Land")).Sum(i => i.Amount);

            DrawLandPct = CardsLeftInDeck == 0 ? 0 : (float)LandsLeftInDeck / CardsLeftInDeck;

            if (hasCapturedTotals)
                return;

            TotalLandsInitial = LandsLeftInDeck;
            TotalCardsInitial = CardsLeftInDeck;
            hasCapturedTotals = true;
        }

        internal void Reset(ICollection<LibraryCardWithAmountVM> deck)
        {
            hasCapturedTotals = false;
            Refresh(deck);
        }

        internal void Reset()
        {
            hasCapturedTotals = false;

            CardsLeftInDeck = 0;
            LandsLeftInDeck = 0;
            DrawLandPct = 0;
            TotalLandsInitial = 0;
            TotalCardsInitial = 0;
        }
    }
}