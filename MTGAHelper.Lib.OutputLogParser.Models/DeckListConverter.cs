﻿using MTGAHelper.Entity;
using MTGAHelper.Lib.Cache;
using Serilog;
using System.Collections.Generic;
using AutoMapper;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class DeckListConverter : IValueConverter<IList<int>, Dictionary<int, int>>
    {
        readonly Dictionary<int, Card> dictAllCards;

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
