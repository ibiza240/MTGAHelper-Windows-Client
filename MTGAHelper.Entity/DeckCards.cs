using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTGAHelper.Entity
{
    public class DeckCards
    {
        public ICollection<DeckCard> All { get; protected set; }
        public ICollection<DeckCard> AllExceptBasicLands { get; protected set; }
        public Dictionary<int, DeckCard> QuickCardsMain { get; protected set; }
        public Dictionary<int, DeckCard> QuickCardsSideboard { get; protected set; }
        public DeckCard QuickCardCommander { get; protected set; }

        public DeckCards(ICollection<DeckCard> Cards)
        {
            this.All = Cards
                //.OrderBy(i => i.Card.cmc)
                //.ThenBy(i => i.Card.name)
                .OrderBy(i => i.Card.name)
                .ToArray();

            this.AllExceptBasicLands = All
                .Where(i => i.Card.type.StartsWith("Basic Land") == false)
                .ToArray();

            this.QuickCardsMain = All
                .Where(i => i.Zone == DeckCardZoneEnum.Deck)
                .ToDictionary(i => i.Card.grpId, i => i);
                /*.GroupBy(i => i.Card.grpId).ToDictionary(i => i.Key, i => new DeckCard(new CardWithAmount(i.First().Card, i.Sum(x => x.Amount)), false))*/;

            this.QuickCardsSideboard = All
                .Where(i => i.Zone == DeckCardZoneEnum.Sideboard)
                .ToDictionary(i => i.Card.grpId, i => i);

            QuickCardCommander = All.FirstOrDefault(i => i.Zone == DeckCardZoneEnum.Commander);
        }
    }
}
