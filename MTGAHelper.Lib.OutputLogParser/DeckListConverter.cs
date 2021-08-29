using System.Collections.Generic;
using AutoMapper;
using MTGAHelper.Entity;
using Serilog;

namespace MTGAHelper.Lib.OutputLogParser
{
    public class DeckListConverter : IValueConverter<IList<int>, Dictionary<int, int>>
    {
        private readonly Dictionary<int, Card> dictAllCards;

        public DeckListConverter(CacheSingleton<Dictionary<int, Card>> cacheCards)
        {
            dictAllCards = cacheCards.Get();
        }

        public ICollection<CardWithAmount> ConvertCards(IList<int> cardsInfo)
        {
            var cards = new List<CardWithAmount>();
            var iCard = 0;
            while (iCard < cardsInfo.Count)
            {
                var grpId = cardsInfo[iCard];
                var amount = cardsInfo[iCard + 1];
                cards.Add(new CardWithAmount(dictAllCards[grpId], amount));
                iCard += 2;
            }

            return cards;
        }

        public Dictionary<int, int> ConvertSimple(IList<int> cardsInfo)
        {
            var cards = new Dictionary<int, int>();

            if (cardsInfo == default)
                return cards;

            var iCard = 0;
            while (iCard < cardsInfo.Count)
            {
                var grpId = cardsInfo[iCard];
                var amount = cardsInfo[iCard + 1];

                if (cards.ContainsKey(grpId))
                {
                    Log.Error("STRANGE! grpId {grpId} was found multiple times in DeckListConverter.ConvertSimple with cardsInfo: <cardsInfo>", grpId, string.Join(",", cardsInfo));
                    cards[grpId] += amount;
                }
                else
                    cards.Add(grpId, amount);

                iCard += 2;
            }

            return cards;
        }

        public Dictionary<int, int> Convert(IList<int> sourceMember, ResolutionContext context)
        {
            return ConvertSimple(sourceMember);
        }
    }
}