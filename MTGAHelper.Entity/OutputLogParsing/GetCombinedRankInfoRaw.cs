using System;
using System.Collections.Generic;
using MTGAHelper.Lib.Config.Users;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public class GetCombinedRankInfoRaw
    {
        public string playerId { get; set; }
        public int constructedSeasonOrdinal { get; set; }
        public string constructedClass { get; set; }
        public int constructedLevel { get; set; }
        public int constructedStep { get; set; }
        public int constructedMatchesWon { get; set; }
        public int constructedMatchesLost { get; set; }
        public int constructedMatchesDrawn { get; set; }
        public int limitedSeasonOrdinal { get; set; }
        public string limitedClass { get; set; }
        public int limitedLevel { get; set; }
        public int limitedStep { get; set; }
        public int limitedMatchesWon { get; set; }
        public int limitedMatchesLost { get; set; }
        public int limitedMatchesDrawn { get; set; }
        public float constructedPercentile { get; set; }
        public int constructedLeaderboardPlace { get; set; }
        public float limitedPercentile { get; set; }
        public int limitedLeaderboardPlace { get; set; }

        public List<ConfigModelRankInfo> ToConfig()
        {
            return new List<ConfigModelRankInfo>
            {
                new ConfigModelRankInfo(RankFormatEnum.Constructed)
                {
                    SeasonOrdinal = constructedSeasonOrdinal,
                    Class = constructedClass,
                    Level = constructedLevel,
                    Step = constructedStep,
                    MatchesWon = constructedMatchesWon,
                    MatchesLost = constructedMatchesLost,
                    MatchesDrawn = constructedMatchesDrawn,
                    Percentile = constructedPercentile,
                    LeaderboardPlace = constructedLeaderboardPlace,
                },
                new ConfigModelRankInfo(RankFormatEnum.Limited)
                {
                    SeasonOrdinal = limitedSeasonOrdinal,
                    Class = limitedClass,
                    Level = limitedLevel,
                    Step = limitedStep,
                    MatchesWon = limitedMatchesWon,
                    MatchesLost = limitedMatchesLost,
                    MatchesDrawn = limitedMatchesDrawn,
                    Percentile = limitedPercentile,
                    LeaderboardPlace = limitedLeaderboardPlace,
                },
            };
        }
    }
}
