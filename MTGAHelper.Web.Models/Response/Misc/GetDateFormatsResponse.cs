using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.Models.Response.Misc
{
    public class GetDateFormatsResponse
    {
        public IReadOnlyCollection<string> DateFormats { get; set; }

        public GetDateFormatsResponse()
        {
        }

        public GetDateFormatsResponse(IReadOnlyCollection<string> dateFormats)
        {
            DateFormats = dateFormats;
        }
    }
}
