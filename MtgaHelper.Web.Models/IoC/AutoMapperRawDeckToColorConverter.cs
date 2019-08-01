using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGAHelper.Web.UI.Shared
{
    public class AutoMapperRawDeckToColorConverter : IValueConverter<ConfigModelRawDeck, string>
    {
        Dictionary<int, Card> dictAllCards;

        public AutoMapperRawDeckToColorConverter(CacheSingleton<ICollection<Card>> cacheCards)
        {
            this.dictAllCards = cacheCards.Get().ToDictionary(i => i.grpId, i => i);
        }

        public string Convert(ConfigModelRawDeck sourceMember, ResolutionContext context)
        {
            var cards = sourceMember.CardsMain.Keys//.Union(sourceMember.CardsSideboard.Keys)
                .Select(i => new DeckCard(new CardWithAmount(dictAllCards[i]), false))
                .ToArray();

            var deck = new Deck(sourceMember.Name, null, cards);
            return deck.GetColor();
        }
    }
}
