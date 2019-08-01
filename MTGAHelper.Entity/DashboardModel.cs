using MTGAHelper.Entity;
using MTGAHelper.Lib.CollectionDecksCompare;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Lib
{
    public class DashboardModel
    {
        public CardMissingDetailsModel[] Details { get; set; } = new CardMissingDetailsModel[0];
        public Dictionary<string, InfoCardMissingSummary[]> Summary { get; set; } = new Dictionary<string, InfoCardMissingSummary[]>();
    }
}
