using System.Collections.Generic;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Web.Models;
using MTGAHelper.Web.UI.Model.Response.Dto;

namespace MTGAHelper.Web.UI.Shared
{
    public class AutoMapperManaCurveConverter : IValueConverter<Dictionary<int, int>, ICollection<DeckManaCurveDto>>
    {
        readonly UtilManaCurve utilManaCurve;

        public AutoMapperManaCurveConverter(UtilManaCurve utilManaCurve)
        {
            this.utilManaCurve = utilManaCurve;
        }

        public ICollection<DeckManaCurveDto> Convert(Dictionary<int, int> sourceMember, ResolutionContext context)
        {
            return utilManaCurve.CalculateManaCurve(context.Mapper.Map<ICollection<CardWithAmount>>(sourceMember));
        }
    }
}
