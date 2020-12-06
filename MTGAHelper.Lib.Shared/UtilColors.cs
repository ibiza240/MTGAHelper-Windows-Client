using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MTGAHelper.Entity;

namespace MTGAHelper.Lib
{
    public class UtilColors : IValueConverter<ICollection<int>, string>, IValueConverter<IDeck, string>
    {
        // TODO: move out of Entity
        readonly Dictionary<string, int> order = new Dictionary<string, int> {
            { "W", 1 },
            { "U", 2 },
            { "B", 3 },
            { "R", 4 },
            { "G", 5 },
        };

        readonly Dictionary<int, Card> dictAllCards;

        public UtilColors(CacheSingleton<Dictionary<int, Card>> allCards)
        {
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
            var (lands, nonLands) = cards.SplitBy(i => i.type.Contains("Land"));
            var landsColors = lands
                .Where(i => i.color_identity != null)
                .SelectMany(i => i.color_identity)
                .Distinct();

            var colors = nonLands
                .Where(i => i.color_identity != null)
                .SelectMany(i => i.color_identity)
                .Distinct()
                .Where(i => landsColors.Contains(i))
                .OrderBy(i => order[i]);

            return colors;
        }

        public string Convert(ICollection<int> sourceMember, ResolutionContext context)
        {
            return FromGrpIds(sourceMember);
        }

        public string Convert(IDeck sourceMember, ResolutionContext context)
        {
            return FromDeck(sourceMember);
        }
    }
}
