using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public class MythicRatingUpdatedRaw
    {
        public double oldMythicPercentile { get; set; }
        public double newMythicPercentile { get; set; }
        //public double newMythicLeaderboardPlacement { get; set; }
        //public string context { get; set; }
    }
}
