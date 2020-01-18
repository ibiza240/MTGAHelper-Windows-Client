using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Response.User
{
    public class DeckTrackedSummaryResponseDto : DeckSummaryResponseDto
    {
        public float MissingWeight { get; set; }
        public float MissingWeightBase { get; set; }
        public float PriorityFactor { get; set; }
        public Dictionary<string, int> WildcardsMissingMain { get; set; }
        public Dictionary<string, int> WildcardsMissingSideboard { get; set; }
    }
}
