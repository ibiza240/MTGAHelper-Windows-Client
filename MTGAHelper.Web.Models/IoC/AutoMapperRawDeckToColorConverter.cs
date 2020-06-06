using System;
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

        public string Convert(ConfigModelRawDeck sourceMember, ResolutionContext context)
        {
            if (sourceMember?.CardsMain == null)
                return "";

            try
            {
                var cards = sourceMember.CardsMainWithCommander.Keys; //.Union(sourceMember.CardsSideboard.Keys)

                return utilColors.FromGrpIds(cards);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"ERROR: whats null? <{sourceMember}> <{sourceMember?.CardsMain}> <{sourceMember?.CardsMain?.Keys}>");
                return "";
            }
        }
    }
}
