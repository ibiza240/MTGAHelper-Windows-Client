using MTGAHelper.Entity;
using MTGAHelper.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTGAHelper.Web.UI.Model.Response
{
    public class GetUserWeightsResponse
    {
        public Dictionary<RarityEnum, UserWeightDto> Weights;

        public GetUserWeightsResponse()
        {
        }

        public GetUserWeightsResponse(Dictionary<RarityEnum, UserWeightDto> weights)
        {
            Weights = weights;
        }
    }
}
