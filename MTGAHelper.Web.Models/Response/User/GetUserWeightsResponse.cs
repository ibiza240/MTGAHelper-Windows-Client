using MTGAHelper.Entity;
using MTGAHelper.Lib;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.UI.Model.Response
{
    public class GetUserWeightsResponse
    {
        public Dictionary<string, UserWeightDto> Weights { get; set; }

        public GetUserWeightsResponse()
        {
        }

        public GetUserWeightsResponse(Dictionary<RarityEnum, UserWeightDto> weights)
        {
            Weights = weights.ToDictionary(i => i.Key.ToString(), i => i.Value);
        }
    }
}
