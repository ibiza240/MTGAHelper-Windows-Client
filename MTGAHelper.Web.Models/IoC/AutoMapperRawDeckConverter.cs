using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib;
using MTGAHelper.Web.UI.Model.Response.Dto;
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

    public class AutoMapperDictCardsByZoneConverter : ITypeConverter<Dictionary<DeckCardZoneEnum, ICollection<DeckCardRaw>>, ICollection<KeyValuePair<string, DeckCardDto[]>>>
    {
        public ICollection<KeyValuePair<string, DeckCardDto[]>> Convert(
            Dictionary<DeckCardZoneEnum, ICollection<DeckCardRaw>> source,
            ICollection<KeyValuePair<string, DeckCardDto[]>> destination, ResolutionContext context
            )
        {
            return source.Select(i => new KeyValuePair<string, DeckCardDto[]>(i.Key.ToString(),
                context.Mapper.Map<DeckCardDto[]>(context.Mapper.Map<ICollection<DeckCard>>(i.Value).ToArray()))).ToArray();
        }
    }
}
