using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.Cache;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.UI.Shared
{
    public class AutoMapperRawDeckToColorConverter : IValueConverter<ConfigModelRawDeck, string>
    {
        readonly Dictionary<int, Card> dictAllCards;
        readonly UtilColors utilColors;

        public AutoMapperRawDeckToColorConverter(CacheSingleton<Dictionary<int, Card>> cacheCards, UtilColors utilColors)
        {
            this.dictAllCards = cacheCards.Get();
            this.utilColors = utilColors;
        }

        public string Convert(ConfigModelRawDeck sourceMember, ResolutionContext context)
        {
            if (sourceMember?.CardsMain == null)
                return "";

            try
            {
                var cards = sourceMember.CardsMainWithCommander.Keys//.Union(sourceMember.CardsSideboard.Keys)
                    .Where(i => dictAllCards.ContainsKey(i))
                    .Select(i => new DeckCard(new CardWithAmount(dictAllCards[i]), DeckCardZoneEnum.Deck))
                    .ToArray();

                var deck = new Deck(sourceMember.Name, null, cards);
                return utilColors.FromDeck(deck);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"ERROR: whats null? <{sourceMember}> <{sourceMember?.CardsMain}> <{sourceMember?.CardsMain?.Keys}>");
                return "";
            }
        }
    }
}
