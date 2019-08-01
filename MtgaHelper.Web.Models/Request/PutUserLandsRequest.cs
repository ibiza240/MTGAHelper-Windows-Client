using MTGAHelper.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGAHelper.Web.UI.Model.Request
{
    public class PutUserLandsRequest
    {
        public ICollection<int> Lands { get; set; }
    }
}
