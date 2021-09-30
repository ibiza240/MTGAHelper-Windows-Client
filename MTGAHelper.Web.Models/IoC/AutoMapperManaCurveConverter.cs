using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Web.Models.Response.Deck;
using System.Collections.Generic;

namespace MTGAHelper.Web.Models.IoC
{
    public class AutoMapperManaCurveConverter : IValueConverter<ICollection<DeckCardRaw>, ICollection<DeckManaCurveDto>>
    {
        private readonly UtilManaCurve utilManaCurve;

        public AutoMapperManaCurveConverter(UtilManaCurve utilManaCurve)
        {
            this.utilManaCurve = utilManaCurve;
        }

        public ICollection<DeckManaCurveDto> Convert(ICollection<DeckCardRaw> sourceMember, ResolutionContext context)
        {
            return utilManaCurve.CalculateManaCurve(context.Mapper.Map<ICollection<CardWithAmount>>(sourceMember));
        }
    }
}