using MTGAHelper.Entity;
using MTGAHelper.Lib.Config.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Lib.UserHistory
{
    public class Outcomes
    {
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }
    }

    public class HistorySummaryForDate
    {
        // From info
        public DateTime Date { get; set; }
        //public ICollection<ConfigModelRankInfo> RankInfo { get; set; } = new ConfigModelRankInfo[0];
        public string ConstructedRank { get; set; }
        public string LimitedRank { get; set; }
        //public int Wins { get; set; }
        //public int Losses { get; set; }
        public Dictionary<string, Outcomes> OutcomesByMode { get; set; }

        // From diff
        public int NewCardsCount { get; set; }
        public int GoldChange { get; set; }
        public int GemsChange { get; set; }
        public float VaultProgressChange { get; set; }
        public Dictionary<RarityEnum, int> WildcardsChange { get; set; } = new Dictionary<RarityEnum, int>
        {
            { RarityEnum.Mythic, 0 },
            { RarityEnum.Rare, 0 },
            { RarityEnum.Uncommon, 0 },
            { RarityEnum.Common, 0 },
        };
    }
}
