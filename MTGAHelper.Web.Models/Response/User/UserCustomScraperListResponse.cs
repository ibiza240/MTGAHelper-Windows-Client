using MTGAHelper.Entity;
using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Response
{
    public class UserCustomScraperListResponse
    {
        public ICollection<ScraperDto> ScrapersByType { get; set; }

        public UserCustomScraperListResponse(ICollection<ScraperDto> scrapers)
        {
            ScrapersByType = scrapers;
        }
    }
}
