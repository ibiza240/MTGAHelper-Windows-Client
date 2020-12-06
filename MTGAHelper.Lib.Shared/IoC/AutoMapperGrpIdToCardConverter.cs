using System.Collections.Generic;
using AutoMapper;
using MTGAHelper.Entity;

namespace MTGAHelper.Lib.IoC
{
    public class AutoMapperGrpIdToCardConverter : ITypeConverter<int, Card>
    {
        readonly Dictionary<int, Card> dictAllCards;

        public AutoMapperGrpIdToCardConverter(CacheSingleton<Dictionary<int, Card>> cacheCards)
        {
            this.dictAllCards = cacheCards.Get();
        }

        public Card Convert(int source, Card destination, ResolutionContext context)
        {
            return dictAllCards.ContainsKey(source)
                ? dictAllCards[source]
                : Card.Unknown;
        }
    }
}
