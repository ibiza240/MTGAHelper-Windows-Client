using System.Collections.Generic;

namespace MTGAHelper.Web.Models.Request
{
    public class PutUserScraperRequest
    {
        public ICollection<string> ScrapersActive { get; set; }
    }
}