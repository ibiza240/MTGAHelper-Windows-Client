using MTGAHelper.Entity;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.CollectionDecksCompare
{
    public class CardRequiredInfoByDeck
    {
        public string DeckId { get; private set; }

        public int NbRequiredMain { get; private set; }
        public int NbRequiredSideboard { get; private set; }
        public int NbOwned { get; private set; }
        public float MissingWeight { get; private set; }
        public int NbMissingMain { get; private set; }
        public int NbMissingSideboard { get; private set; }
        public bool IsTracked { get; private set; }
        public Dictionary<string, CardRequiredInfoByCard> ByCard { get; private set; }

        public int NbMissing => NbMissingMain + NbMissingSideboard;
        public int NbRequired => NbRequiredMain + NbRequiredSideboard;

        public CardRequiredInfoByDeck(string deckId, IEnumerable<CardRequiredInfo> infos, Dictionary<string, bool> decksTracked)
        {
            //try
            //{
                DeckId = deckId;

                var cards = infos
                    .GroupBy(i => i.Card.name)
                    .Select(i => new CardRequiredInfoByCard(i, decksTracked, false))
                    .ToArray();

                NbRequiredMain = cards.Sum(i => i.NbRequiredMain);
                NbRequiredSideboard = cards.Sum(i => i.NbRequiredSideboard);
                NbOwned = cards.Sum(i => i.NbOwned);
                NbMissingMain = cards.Sum(i => i.NbMissingMain);
                NbMissingSideboard = cards.Sum(i => i.NbMissingSideboard);
                IsTracked = decksTracked[deckId];

                //if (deckId == "bd100c44f66d3c0c3419deac499ef60ffc7e6227be93c034bb97b9c77ed6ca2f")
                //    System.Diagnostics.Debugger.Break();

                MissingWeight = cards.Sum(i => i.MissingWeight);

                ByCard = cards.ToDictionary(i => i.Card.name, i => i);
            //}
            //catch(System.Exception ex)
            //{
            //    System.Diagnostics.Debugger.Break();
            //    throw;
            //}
        }
    }
}
