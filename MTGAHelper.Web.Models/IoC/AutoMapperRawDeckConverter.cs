using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib;
using MTGAHelper.Web.Models.Response.Deck;
using MTGAHelper.Web.Models.SharedDto;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.Models.IoC
{
    public class AutoMapperRawDeckConverter : ITypeConverter<IReadOnlyDictionary<int, int>, ICollection<CardWithAmountDto>>
    {
        private readonly RawDeckConverter converter;

        public AutoMapperRawDeckConverter(RawDeckConverter converter)
        {
            this.converter = converter;
        }

        public ICollection<CardWithAmountDto> Convert(IReadOnlyDictionary<int, int> source, ICollection<CardWithAmountDto> destination, ResolutionContext context)
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