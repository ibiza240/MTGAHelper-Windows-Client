using MTGAHelper.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.CollectionDecksCompare
{
    public class CardRequiredInfoByCard
    {
        public Card Card { get; private set; }

        public int NbRequiredMain { get; private set; }
        public int NbRequiredSideboard { get; private set; }
        public int NbOwned { get; private set; }
        public float MissingWeight { get; private set; }
        public int NbMissingMain { get; private set; }
        public int NbMissingSideboard { get; private set; }
        public Dictionary<string, CardRequiredInfoByDeck> ByDeck { get; private set; }

        public int NbRequired { get { return NbRequiredMain + NbRequiredSideboard; } }
        public int NbMissing { get { return ByDeck.Any() ? ByDeck.Max(i => i.Value.NbMissing) : 0; } }
        public int NbDecks { get { return ByDeck?.Count ?? 0; } }

        public CardRequiredInfoByCard(IEnumerable<CardRequiredInfo> cards, Dictionary<string, bool> decksTracked, bool buildByDeck = true)
        {
            Card = cards.First().Card;
            //if (Card.name == "Duress") System.Diagnostics.Debugger.Break();

            if (cards.Any(i => i.Card.name != Card.name))
            {
                throw new Exception("ByCard not with all the same card.");
            }

            //if (cards.Any(c => c.Card.name.StartsWith("Verix")))
            //{
            //    System.Diagnostics.Debugger.Break();
            //}

            NbOwned = cards.First().NbOwned;

            var nbPerDeckMain = cards
                .Where(i => i.IsSideboard == false && i.IsForAverageArchetypeOthersInMain == false)
                .GroupBy(i => i.DeckId)
                .Select(i => i.Sum(x => x.NbRequired))
                .ToArray();
            NbRequiredMain = nbPerDeckMain.Length == 0 ? 0 : Math.Min(4, nbPerDeckMain.Max());
            NbMissingMain = Math.Max(0, NbRequiredMain - NbOwned);

            var nbPerDeckSideboard = cards
                .Where(i => i.IsSideboard == true)
                .GroupBy(i => i.DeckId)
                .Select(i => i.Sum(x => x.NbRequired))
                .ToArray();
            NbRequiredSideboard = nbPerDeckSideboard.Length == 0 ? 0 : Math.Min(4, nbPerDeckSideboard.Max());

            if (NbRequiredMain + NbRequiredSideboard > 4)
            {
                //System.Diagnostics.Debugger.Break();
                NbRequiredSideboard = 4 - NbRequiredMain;
            }

            NbMissingSideboard = Math.Max(0, NbRequiredSideboard - NbOwned + NbRequiredMain - NbMissingMain);

            MissingWeight = cards.Sum(i => i.MissingWeight);

            if (buildByDeck)
            {
                ByDeck = cards
                    //.Where(i => i.MissingWeight > 0 || i.Card.type.Contains("Land"))
                    .GroupBy(x => x.DeckId)
                    .ToDictionary(x => x.Key, x => new CardRequiredInfoByDeck(x.Key, x, decksTracked));
            }
        }
    }
}
