using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGAHelper.Web.UI.Model.Request
{
    public class PostUserCustomDeckRequest
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string MtgaFormat { get; set; }
    }
}
