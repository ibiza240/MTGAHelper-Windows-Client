using MTGAHelper.Lib;
using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Request
{
    public class PutUserWeightsRequest
    {
        public Dictionary<string, UserWeightDto> Weights { get; set; }
    }
}
