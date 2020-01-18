using MTGAHelper.Entity;
using System.Collections.Generic;

namespace MTGAHelper.Web.Models.Response.Misc
{
    public class DraftRatingsResponse
    {
        public bool IsUpToDate { get; set; }
        public Dictionary<string, DraftRatings> Ratings { get; set; }
    }
}
