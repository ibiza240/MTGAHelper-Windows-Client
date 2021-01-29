using System.Collections.Generic;
using MTGAHelper.Entity.CollectionDecksCompare;

namespace MTGAHelper.Entity
{
    public class DashboardModel
    {
        public CardMissingDetailsModel[] Details { get; set; } = new CardMissingDetailsModel[0];
        public Dictionary<string, DashboardModelSummary> Summary { get; set; } = new Dictionary<string, DashboardModelSummary>();
    }

    public class DashboardModelSummary
    {
        public float ExpectedValue { get; set; }
        public InfoCardMissingSummary[] Data { get; set; }
    }
}
