using System;
using System.Collections.Generic;
using MTGAHelper.Lib.Config.Users;

namespace MTGAHelper.Entity.OutputLogParsing
{
    //public class LocParams
    //{
    //    public int number1 { get; set; }
    //    public int number2 { get; set; }
    //}

    //public class ChestDescription
    //{
    //    public string image1 { get; set; }
    //    public string prefab { get; set; }
    //    public string headerLocKey { get; set; }
    //    public string quantity { get; set; }
    //    public LocParams locParams { get; set; }
    //    public DateTime availableDate { get; set; }
    //}

    public class QuestUpdate
    {
        public string questId { get; set; }
        public int goal { get; set; }
        public string locKey { get; set; }
        public string tileResourceId { get; set; }
        public string treasureResourceId { get; set; }
        public string questTrack { get; set; }
        public bool isNewQuest { get; set; }
        public int endingProgress { get; set; }
        public int startingProgress { get; set; }
        public bool canSwap { get; set; }
        public ChestDescription chestDescription { get; set; }
        public int hoursWaitAfterComplete { get; set; }
    }

    public class BoosterDelta
    {
        public int collationId { get; set; }
        public int count { get; set; }
    }

    //public class ArtSkin
    //{
    //    public int artId { get; set; }
    //    public string ccv { get; set; }
    //}

    public class Delta
    {
        public List<int> cardsAdded { get; set; } = new List<int>();
        public List<BoosterDelta> boosterDelta { get; set; } = new List<BoosterDelta>();
        public List<object> decksAdded { get; set; } = new List<object>();
        public List<object> starterDecksAdded { get; set; } = new List<object>();
        public List<object> voucherItemsDelta { get; set; } = new List<object>();
        public int wcCommonDelta { get; set; }
        public int wcUncommonDelta { get; set; }
        public int wcRareDelta { get; set; }
        public int wcMythicDelta { get; set; }
        public int goldDelta { get; set; }
        public int gemsDelta { get; set; }
        public int earnedGemsDelta { get; set; }
        public int draftTokensDelta { get; set; }
        public int sealedTokensDelta { get; set; }
        public int wcTrackPosition { get; set; }
        public float vaultProgressDelta { get; set; }
        public int newNValCommon { get; set; }
        public int newNValUncommon { get; set; }
        public int newNValRare { get; set; }
        public int newNValMythic { get; set; }
        public List<object> vanityItemsAdded { get; set; } = new List<object>();
        public List<object> vanityItemsRemoved { get; set; } = new List<object>();
        public List<CardSkin> artSkinsAdded { get; set; } = new List<CardSkin>();
        public List<CardSkin> artSkinsRemoved { get; set; } = new List<CardSkin>();
        public object basicLandSet { get; set; }
        public string invEtag { get; set; }
        public string cardEtag { get; set; }
        public string cosmeticEtag { get; set; }
        public int storeBoostersOpened { get; set; }
    }

    //public class Context
    //{
    //    public string source { get; set; }
    //    public string sourceId { get; set; }
    //}

    //public class DailyWinUpdate
    //{
    //    public Delta delta { get; set; }
    //    public List<AetherizedCard> aetherizedCards { get; set; }
    //    public int xpGained { get; set; }
    //    public Context context { get; set; }
    //}

    //public class Delta2
    //{
    //    public int gemsDelta { get; set; }
    //    public int goldDelta { get; set; }
    //    public List<object> boosterDelta { get; set; }
    //    public List<object> cardsAdded { get; set; }
    //    public List<object> decksAdded { get; set; }
    //    public List<object> starterDecksAdded { get; set; }
    //    public List<object> vanityItemsAdded { get; set; }
    //    public List<object> vanityItemsRemoved { get; set; }
    //    public int draftTokensDelta { get; set; }
    //    public int sealedTokensDelta { get; set; }
    //    public double vaultProgressDelta { get; set; }
    //    public int wcCommonDelta { get; set; }
    //    public int wcUncommonDelta { get; set; }
    //    public int wcRareDelta { get; set; }
    //    public int wcMythicDelta { get; set; }
    //    public List<object> artSkinsAdded { get; set; }
    //    public List<object> artSkinsRemoved { get; set; }
    //    public List<object> voucherItemsDelta { get; set; }
    //}

    //public class Context2
    //{
    //    public string source { get; set; }
    //    public string sourceId { get; set; }
    //}

    //public class TimeframeWinUpdate
    //{
    //    public Delta delta { get; set; }
    //    public List<AetherizedCard> aetherizedCards { get; set; }
    //    public int xpGained { get; set; }
    //    public Context context { get; set; }
    //}

    public class TrackDiff
    {
        public int currentLevel { get; set; }
        public int currentExp { get; set; }
        public int oldLevel { get; set; }
        public int oldExp { get; set; }
        public List<Update> inventoryUpdates { get; set; } = new List<Update>();
    }

    public class RewardWebDiff
    {
        public List<int> currentUnlockedNodes { get; set; }
        public List<int> currentAvailableNodes { get; set; }
        public List<int> oldUnlockedNodes { get; set; }
        public List<int> oldAvailableNodes { get; set; }
        public List<Update> inventoryUpdates { get; set; }
    }

    public class OrbCountDiff
    {
        public int oldOrbCount { get; set; }
        public int currentOrbCount { get; set; }
    }

    //public class EppUpdate
    //{
    //    public string trackName { get; set; }
    //    public int trackTier { get; set; }
    //    public TrackDiff trackDiff { get; set; }
    //    public RewardWebDiff rewardWebDiff { get; set; }
    //    public OrbCountDiff orbCountDiff { get; set; }
    //}

    //public class TrackDiff2
    //{
    //    public int currentLevel { get; set; }
    //    public int currentExp { get; set; }
    //    public int oldLevel { get; set; }
    //    public int oldExp { get; set; }
    //    public List<object> inventoryUpdates { get; set; }
    //}

    //public class RewardWebDiff2
    //{
    //    public List<int> currentUnlockedNodes { get; set; }
    //    public List<int> currentAvailableNodes { get; set; }
    //    public List<int> oldUnlockedNodes { get; set; }
    //    public List<int> oldAvailableNodes { get; set; }
    //    public List<object> inventoryUpdates { get; set; }
    //}

    //public class OrbCountDiff2
    //{
    //    public int oldOrbCount { get; set; }
    //    public int currentOrbCount { get; set; }
    //}

    public class BattlePassUpdate
    {
        public string trackName { get; set; }
        public int trackTier { get; set; }
        public TrackDiff trackDiff { get; set; } = new TrackDiff();
        public RewardWebDiff rewardWebDiff { get; set; } = new RewardWebDiff();
        public OrbCountDiff orbCountDiff { get; set; } = new OrbCountDiff();
    }

    public class PostMatchUpdateRaw
    {
        public List<QuestUpdate> questUpdate { get; set; } = new List<QuestUpdate>();
        public List<Update> dailyWinUpdates { get; set; } = new List<Update>();
        public List<Update> weeklyWinUpdates { get; set; } = new List<Update>();
        public BattlePassUpdate eppUpdate { get; set; } = new BattlePassUpdate();
        public BattlePassUpdate battlePassUpdate { get; set; } = new BattlePassUpdate();
    }
}
