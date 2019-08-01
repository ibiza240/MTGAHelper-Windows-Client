using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGAHelper.Web.UI.Model.Request
{
    public class PatchDeckPriorityFactorRequest
    {
        public string DeckId { get; set; }
        public float  Value { get; set; }
    }
}
