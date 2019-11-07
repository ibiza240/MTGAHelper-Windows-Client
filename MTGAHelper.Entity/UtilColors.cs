using MTGAHelper.Lib.Cache;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Entity
{
    public class UtilColors
    {
        Dictionary<string, int> order = new Dictionary<string, int> {
            { "W", 1 },
            { "U", 2 },
            { "B", 3 },
            { "R", 4 },
            { "G", 5 },
        };

        public Dictionary<int, Card> dictAllCards;

        public UtilColors(CacheSingleton<Dictionary<int, Card>> allCards)
        {
            //var value = allCards.Get();
            //if (value != null)
            //    dictAllCards = value.ToDictionary(i => i.grpId, i => i);
            dictAllCards = allCards.Get();
        }

        public string FromGrpIds(ICollection<int> grpIds)
        {
            var colors = GetColorFromCards(grpIds
                .Where(i => dictAllCards.ContainsKey(i))
                .Select(i => dictAllCards[i]));
            return string.Join("", colors);
        }

        public string FromDeck(IDeck deck)
        {
            var colors = GetColorFromCards(deck.Cards.All.Select(i => i.Card));
            return string.Join("", colors);
        }

        public string FromCards(ICollection<Card> cards)
        {
            var colors = GetColorFromCards(cards);
            return string.Join("", colors);
        }

        IEnumerable<string> GetColorFromCards(IEnumerable<Card> cards)
        {
            var landsColors = cards.Where(i => i.type.Contains("Land"))
                .Where(i => i.color_identity != null)
                .SelectMany(i => i.color_identity)
                .Distinct();

            var colors = cards
                .Where(i => i.color_identity != null)
                .SelectMany(i => i.color_identity)
                .Distinct()
                .Where(i => landsColors.Contains(i))
                .OrderBy(i => order[i]);

            return colors;
        }
    }
}
