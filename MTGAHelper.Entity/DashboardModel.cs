using System.Collections.Generic;
using MTGAHelper.Entity.CollectionDecksCompare;

namespace MTGAHelper.Entity
{
    public class DashboardModel
    {
        public CardMissingDetailsModel[] Details { get; set; } = new CardMissingDetailsModel[0];
        public Dictionary<string, InfoCardMissingSummary[]> Summary { get; set; } = new Dictionary<string, InfoCardMissingSummary[]>();
    }
}
