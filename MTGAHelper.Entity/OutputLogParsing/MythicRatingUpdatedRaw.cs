using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public class MythicRatingUpdatedRaw
    {
        public float oldMythicPercentile { get; set; }
        public float newMythicPercentile { get; set; }
        public float oldMythicLeaderboardPlacement { get; set; }
        public float newMythicLeaderboardPlacement { get; set; }
        public string context { get; set; }
    }
}
