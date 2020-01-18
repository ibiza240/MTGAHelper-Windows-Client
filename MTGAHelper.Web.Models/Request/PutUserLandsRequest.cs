using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Request
{
    public class PutUserLandsRequest
    {
        public ICollection<int> Lands { get; set; }
    }
}
