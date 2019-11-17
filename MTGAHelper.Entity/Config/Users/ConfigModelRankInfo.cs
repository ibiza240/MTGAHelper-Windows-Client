namespace MTGAHelper.Lib.Config.Users
{
    public enum ConfigModelRankInfoFormatEnum
    {
        Unknown,
        Constructed,
        Limited
    }

    public class ConfigModelRankInfo
    {
        public const string UNKNOWN = "_0_0";

        public ConfigModelRankInfoFormatEnum Format { get; set; }
        public int SeasonOrdinal { get; set; }
        public string Class { get; set; }
        public int Level { get; set; }
        public int Step { get; set; }
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

        public ConfigModelRankInfo(ConfigModelRankInfoFormatEnum format)
        {
            Format = format;
        }
    }
}
