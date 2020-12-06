using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Entity
{
    public class DeckCards
    {
        public ICollection<DeckCard> All { get; protected set; }
        public ICollection<DeckCard> AllExceptBasicLands { get; protected set; }
        public Dictionary<int, DeckCard> QuickCardsMain { get; protected set; }
        public Dictionary<int, DeckCard> QuickCardsSideboard { get; protected set; }
        public DeckCard QuickCardCommander { get; protected set; }
        public DeckCard QuickCardCompanion { get; protected set; }
        public string ColorsInDeck { get; protected set; }

        public DeckCards(IEnumerable<DeckCard> cards)
        {
            All = cards
                //.OrderBy(i => i.Card.cmc)
                //.ThenBy(i => i.Card.name)
                .OrderBy(i => i.Card.name)
                .ToArray();

            AllExceptBasicLands = All
                .Where(i => i.Card.type.StartsWith("Basic Land") == false)
                .ToArray();

            QuickCardsMain = All
                .Where(i => i.Zone == DeckCardZoneEnum.Deck)
                .ToDictionary(i => i.Card.grpId, i => i);

            QuickCardsSideboard = All
                .Where(i => i.Zone == DeckCardZoneEnum.Sideboard)
                .ToDictionary(i => i.Card.grpId, i => i);

            var colorsInDeck = All.Select(c => c.Card)
                .Where(c => c.colors != null && c.colors.Any())
                .SelectMany(c => c.colors)
                .Distinct()
                .OrderBy(c => c);
            ColorsInDeck = string.Concat(colorsInDeck);

            QuickCardCommander = All.FirstOrDefault(i => i.Zone == DeckCardZoneEnum.Commander);

            QuickCardCompanion = All.FirstOrDefault(i => i.Zone == DeckCardZoneEnum.Companion);
        }
    }
}
