namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class MatchCreatedRaw
    {
        public string controllerFabricUri { get; set; }
        public string matchEndpointHost { get; set; }
        public int matchEndpointPort { get; set; }
        public string opponentScreenName { get; set; }
        public bool opponentIsWotc { get; set; }
        public string matchId { get; set; }
        public string opponentRankingClass { get; set; }
        public int opponentRankingTier { get; set; }
        public double opponentMythicPercentile { get; set; }
        public int opponentMythicLeaderboardPlace { get; set; }
        public string eventId { get; set; }
        public string opponentAvatarSelection { get; set; }
    }
}
