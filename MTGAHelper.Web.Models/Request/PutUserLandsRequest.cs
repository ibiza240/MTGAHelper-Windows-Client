using System.Collections.Generic;

namespace MTGAHelper.Web.Models.Request
{
    public class PutUserLandsRequest
    {
        public ICollection<int> Lands { get; set; }
    }
}