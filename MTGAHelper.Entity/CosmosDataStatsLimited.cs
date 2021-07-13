using MTGAHelper.Entity.UserHistory;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    public class ResultsInventoryChange
    {
        public int Amount { get; set; }
        public EconomyEventChangeEnum Type { get; set; }
    }

    public class LimitedEventResults
    {
        public string Set { get; set; }
        public string DraftType { get; set; }
        public ResultsInventoryChange EntryFee { get; set; }
        public ICollection<ResultsInventoryChange> Rewards { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int WinCount { get; set; }
        public int LossCount { get; set; }
        public int DrawCount { get; set; }

        public int TotalCount => WinCount + LossCount;
    }

    public class CosmosDataStatsLimited
    {
        public bool IsUpToDate { get; set; }
        public Dictionary<string, ICollection<LimitedEventResults>> Stats { get; set; }
    }
}
