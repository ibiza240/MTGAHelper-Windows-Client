using System.Collections.Generic;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.Cache;

namespace MTGAHelper.Web.UI.Shared
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
                : new Card
                {
                    grpId = 0,
                    name = "Unknown",
                    imageCardUrl =
                        "https://cdn11.bigcommerce.com/s-0kvv9/images/stencil/1280x1280/products/266486/371622/classicmtgsleeves__43072.1532006814.jpg?c=2&imbypass=on"
                };
        }
    }
}
