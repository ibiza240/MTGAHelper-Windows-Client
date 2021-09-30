using MTGAHelper.Entity;
using System.Collections.Generic;

namespace MTGAHelper.Web.Models.Response.User
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