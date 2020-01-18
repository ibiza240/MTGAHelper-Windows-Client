using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.Cache;
using MTGAHelper.Web.UI.Model.SharedDto;
using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Shared
{
    public class AutoMapperRawDeckConverter : ITypeConverter<Dictionary<int, int>, ICollection<CardWithAmountDto>>
    {
        readonly Dictionary<int, Card> allCards;
        readonly RawDeckConverter converter;

        public AutoMapperRawDeckConverter(CacheSingleton<Dictionary<int, Card>> cacheCards, RawDeckConverter converter)
        {
            //if (allCards == null)
            //    System.Diagnostics.Debugger.Break();

            this.allCards = cacheCards.Get();
            this.converter = converter;
        }

        public ICollection<CardWithAmountDto> Convert(Dictionary<int, int> source, ICollection<CardWithAmountDto> destination, ResolutionContext context)
        {
            return Mapper.Map<ICollection<CardWithAmountDto>>(converter.Init(allCards).LoadCollection(source));
        }
    }
}
