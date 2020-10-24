using System;
using System.Linq;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib;
using Serilog;

namespace MTGAHelper.Web.UI.Shared
{
    public class AutoMapperRawDeckToColorConverter : IValueConverter<ConfigModelRawDeck, string>
    {
        readonly UtilColors utilColors;

        public AutoMapperRawDeckToColorConverter(UtilColors utilColors)
        {
            this.utilColors = utilColors;
        }

        public string Convert(ConfigModelRawDeck src, ResolutionContext context)
        {
            var cardsMain = src?.Cards?.Where(i => i.Zone == DeckCardZoneEnum.Deck || i.Zone == DeckCardZoneEnum.Commander);

            if (cardsMain == null)
                return "";

            try
            {
                var cards = cardsMain.Select(i => i.GrpId).ToArray();
                return utilColors.FromGrpIds(cards);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"ERROR: whats null? <{src}> <{src?.Cards}> <{string.Join(",", src?.Cards?.Select(i => i.GrpId))}>");
                return "";
            }
        }
    }
}
