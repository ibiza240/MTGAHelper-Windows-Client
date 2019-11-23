using System;
using System.Collections.Generic;
using MTGAHelper.Lib.Config.Users;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class LocParams
    {
        public int number1 { get; set; }
        public int number2 { get; set; }
        public int number3 { get; set; }
    }

    public class RankInfo
    {
        public string image1 { get; set; }
        public string image2 { get; set; }
        public string image3 { get; set; }
        public string prefab { get; set; }
        public string referenceId { get; set; }
        public string headerLocKey { get; set; }
        public string descriptionLocKey { get; set; }
        public string quantity { get; set; }
        public LocParams locParams { get; set; }
        public DateTime availableDate { get; set; }
    }

    public class SeasonRewards
    {
        public RankInfo Bronze { get; set; }
        public RankInfo Silver { get; set; }
        public RankInfo Gold { get; set; }
        public RankInfo Platinum { get; set; }
        public RankInfo Diamond { get; set; }
        public RankInfo Mythic { get; set; }
    }

    public class CurrentSeason
    {
        public int seasonOrdinal { get; set; }
        public DateTime seasonStartTime { get; set; }
        public DateTime seasonEndTime { get; set; }
        public SeasonRewards seasonLimitedRewards { get; set; }
        public SeasonRewards seasonConstructedRewards { get; set; }
        public int minMatches { get; set; }
    }

    public class FormatRankInfo
    {
        public string rankClass { get; set; }
        public int level { get; set; }
        public int steps { get; set; }
    }

    public class GetSeasonAndRankDetailRaw
    {
        public CurrentSeason currentSeason { get; set; }
        public List<FormatRankInfo> limitedRankInfo { get; set; }
        public List<FormatRankInfo> constructedRankInfo { get; set; }
    }
}
