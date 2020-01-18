using MTGAHelper.Entity;
using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Response.Misc
{
    public class GetLandsResponse
    {
        public ICollection<CardLandPreferenceDto> Lands { get; set; }

        public GetLandsResponse(ICollection<CardLandPreferenceDto> lands)
        {
            Lands = lands;
        }
    }
}
