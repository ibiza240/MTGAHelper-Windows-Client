using MTGAHelper.Entity;

namespace MTGAHelper.Lib.Config.Users
{
    public class ConfigModelRankInfo : Rank
    {
        public const string UNKNOWN = "_0_0";

        public int MatchesWon { get; set; }
        public int MatchesLost { get; set; }
        public int MatchesDrawn { get; set; }
        public float Percentile { get; set; }
        public int LeaderboardPlace { get; set; }

        public override string ToString()
        {
            return $"{Class}_{Level}_{Step}";
        }

        public ConfigModelRankInfo()
        {
            // For serialization
        }

        public ConfigModelRankInfo(RankFormatEnum format)
        {
            Format = format;
        }
    }
}
