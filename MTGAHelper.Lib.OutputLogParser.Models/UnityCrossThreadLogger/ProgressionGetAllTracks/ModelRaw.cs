using System;
using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class TrackLevel
    {
        public int xpToComplete { get; set; }
        public bool isPageStarter { get; set; }
        public bool isTentpole { get; set; }
    }

    public class Chest
    {
        public string image1 { get; set; }
        public object image2 { get; set; }
        public object image3 { get; set; }
        public string prefab { get; set; }
        public string referenceId { get; set; }
        public string headerLocKey { get; set; }
        public string descriptionLocKey { get; set; }
        public object quantity { get; set; }
        public LocParams locParams { get; set; }
        public DateTime availableDate { get; set; }
    }

    public class UpgradePacket
    {
        public string targetDeckDescription { get; set; }
        public List<int> cardsAdded { get; set; }
    }

    public class AllNode
    {
        public string id { get; set; }
        public string unlockQuestMetric { get; set; }
        public int unlockMetricCount { get; set; }
        public Chest chest { get; set; }
        public UpgradePacket upgradePacket { get; set; }
        public List<object> childIds { get; set; }
    }

    public class RewardWeb
    {
        public List<AllNode> allNodes { get; set; }
        public List<int> topLevelNodeIds { get; set; }
        public bool enabled { get; set; }
    }

    public class ProgressionGetAllTracksRaw
    {
        public string name { get; set; }
        public List<TrackLevel> trackLevels { get; set; }
        public List<List<TrackRewardTier>> trackRewardTiers { get; set; }
        public int numExtendedLevels { get; set; }
        public int extendedLevelXpToComplete { get; set; }
        public List<object> extendedLevelRewardTiers { get; set; }
        public RewardWeb rewardWeb { get; set; }
        public bool enabled { get; set; }
        public DateTime expirationTime { get; set; }
    }

    public class TrackRewardTier
    {
        public Chest chest { get; set; }
        public int orbsRewarded { get; set; }
    }
}
