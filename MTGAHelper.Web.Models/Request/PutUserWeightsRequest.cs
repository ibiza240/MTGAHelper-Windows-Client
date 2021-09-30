using MTGAHelper.Lib;
using System.Collections.Generic;
using MTGAHelper.Entity;

namespace MTGAHelper.Web.Models.Request
{
    public class PutUserWeightsRequest
    {
        public Dictionary<string, UserWeightDto> Weights { get; set; }
    }
}