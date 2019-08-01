using MTGAHelper.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGAHelper.Web.UI.Model.Request
{
    public class PutUserScraperRequest
    {
        public ICollection<string> ScrapersActive { get; set; }
    }
}
