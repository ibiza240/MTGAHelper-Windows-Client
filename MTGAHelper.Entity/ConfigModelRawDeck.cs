using MTGAHelper.Entity.OutputLogParsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Entity
{
    public class ConfigModelRawDeck
    {
        public string Id { get; set; }
        //public int CommandZoneGrpId { get; set; }
        public int DeckTileId { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Name { get; set; }
        public string Format { get; set; }
        public string ArchetypeId { get; set; }
        public Dictionary<int, int> CardsMain { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> CardsSideboard { get; set; } = new Dictionary<int, int>();
        public int CardCommander { get; set; }

        public Dictionary<int, int> CardsMainWithCommander => CardsMain.Union(CardCommander == default(int) ? new KeyValuePair<int, int>[0] : new[] { new KeyValuePair<int, int>(CardCommander, 1) })
            .ToDictionary(i => i.Key, i => i.Value);

        public string Description { get; set; }
        public string CardBack { get; set; }

        public ICollection<DeckCard> ToDeckCards(Dictionary<int, Card> allCards)
        {
            var cardsMain = CardsMainWithCommander.Select(i => new DeckCard(new CardWithAmount(allCards[i.Key], i.Value), DeckCardZoneEnum.Deck));
            var cardsSideboard = CardsSideboard.Select(i => new DeckCard(new CardWithAmount(allCards[i.Key], i.Value), DeckCardZoneEnum.Sideboard));

            return cardsMain.Union(cardsSideboard).ToArray();
        }
    }
}
