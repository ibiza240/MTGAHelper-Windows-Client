using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Request
{
    public class PutUserScraperRequest
    {
        public ICollection<string> ScrapersActive { get; set; }
    }
}
