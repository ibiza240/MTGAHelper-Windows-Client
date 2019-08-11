using System.Collections.Generic;

namespace MTGAHelper.Web.Models.Response.Misc
{
    public class GetDateFormatsResponse
    {
        public ICollection<string> DateFormats { get; set; }

        public GetDateFormatsResponse(ICollection<string> dateFormats)
        {
            DateFormats = dateFormats;
        }
    }
}
