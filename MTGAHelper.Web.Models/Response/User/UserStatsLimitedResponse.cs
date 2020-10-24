using System;
using System.Collections.Generic;

namespace MTGAHelper.Web.Models.Response.User
{
    public class UserStatsLimitedResponseSummary
    {
        public string Set { get; set; }
        public string DraftType { get; set; }
        public int EventsCount { get; set; }
        public float WinCountAverage { get; set; }
        public float WinRate { get; set; }
    }
    public class UserStatsLimitedResponseEvent
    {
        public string Set { get; set; }
        public string DraftType { get; set; }
        //public string EntryFee { get; set; }
        //public ICollection<ResultsInventoryChange> Rewards { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int WinCount { get; set; }
        public int LossCount { get; set; }
        public int DrawCount { get; set; }

        public int TotalCount => WinCount + LossCount + DrawCount;
    }

    public class UserStatsLimitedResponse
    {
        public ICollection<UserStatsLimitedResponseSummary> Summary { get; set; }
        public ICollection<UserStatsLimitedResponseEvent> Details { get; set; }
    }
}
