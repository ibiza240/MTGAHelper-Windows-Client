using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTGAHelper.Lib.Cache;

namespace MTGAHelper.Entity.IoC
{
    public class MapperProfileEntity : Profile
    {
        public MapperProfileEntity(CacheSingleton<ICollection<Card>> allCards)
        {
            var dictAllCards = allCards.Get().ToDictionary(i => i.grpId, i => i);

            // grpId to Card
            CreateMap<int, Card>().ConvertUsing(i => dictAllCards.ContainsKey(i) ? dictAllCards[i] :
                new Card
                {
                    grpId = 0,
                    name = "Unknown",
                    imageCardUrl = "https://cdn11.bigcommerce.com/s-0kvv9/images/stencil/1280x1280/products/266486/371622/classicmtgsleeves__43072.1532006814.jpg?c=2&imbypass=on"
                });

            //CreateMap<MtgaDeck, ConfigModelRawDeck>();

            CreateMap<KeyValuePair<int, int>, CardWithAmount>()
                .ConvertUsing(i => new CardWithAmount(dictAllCards[i.Key], i.Value));
        }
    }
}
