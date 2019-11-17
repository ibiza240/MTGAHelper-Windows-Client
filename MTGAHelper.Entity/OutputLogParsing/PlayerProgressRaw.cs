using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public class BattlePass
    {
        public string trackName { get; set; }
        public int currentLevel { get; set; }
        public int currentExp { get; set; }
        public List<int> rewardTiers { get; set; }
        public int currentOrbCount { get; set; }
        public List<int> unlockedNodeIds { get; set; }
        public List<int> availableNodesToUnlock { get; set; }
    }

    public class GetPlayerProgressRaw
    {
        public BattlePass eppTrack { get; set; }
        public bool isEppTrackActive { get; set; }
        public BattlePass activeBattlePass { get; set; }
        public List<BattlePass> expiredBattlePasses { get; set; }
        public string currentRenewalId { get; set; }
    }
}
