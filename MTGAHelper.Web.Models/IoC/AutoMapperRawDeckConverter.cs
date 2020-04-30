using System.Collections.Generic;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Web.UI.Model.SharedDto;

namespace MTGAHelper.Web.UI.Shared
{
    public class AutoMapperRawDeckConverter : ITypeConverter<Dictionary<int, int>, ICollection<CardWithAmountDto>>
    {
        readonly RawDeckConverter converter;

        public AutoMapperRawDeckConverter(RawDeckConverter converter)
        {
            this.converter = converter;
        }

        public ICollection<CardWithAmountDto> Convert(Dictionary<int, int> source, ICollection<CardWithAmountDto> destination, ResolutionContext context)
        {
            return context.Mapper.Map<ICollection<CardWithAmountDto>>(converter.LoadCollection(source));
        }
    }
}
