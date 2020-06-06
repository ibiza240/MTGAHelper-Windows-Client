namespace MTGAHelper.Entity.OutputLogParsing
{
    public class MythicRatingUpdatedRaw
    {
        public float oldMythicPercentile { get; set; }
        public float newMythicPercentile { get; set; }
        public int oldMythicLeaderboardPlacement { get; set; }
        public int newMythicLeaderboardPlacement { get; set; }
        public string context { get; set; }
    }
}
