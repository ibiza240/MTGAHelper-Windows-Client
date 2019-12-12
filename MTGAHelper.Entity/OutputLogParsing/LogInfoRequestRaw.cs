using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity.OutputLogParsing
{
    //public interface ILogInfoResponsePayloadObject : ITagPayloadObject
    //{
    //}

    //public class PayloadObject : ITagPayloadObject
    //{

    //}

    // Client.SceneChange
    public class SceneChange_PayloadObject// : ILogInfoResponsePayloadObject
    {
        public string fromSceneName { get; set; }
        public string toSceneName { get; set; }
        public DateTime timestamp { get; set; }
        public string duration { get; set; }
        public string initiator { get; set; }
        public string context { get; set; }
        public string playerId { get; set; }
    }

    // DuelScene.EndOfMatchReport
    public class EndOfMatchReport_PayloadObject// : ILogInfoResponsePayloadObject
    {
        public string matchId { get; set; }
        public int maxCreatures { get; set; }
        public int maxLands { get; set; }
        public int maxArtifactsAndEnchantments { get; set; }
        public string longestPassPriorityWaitTimeInSeconds { get; set; }
        public string shortestPassPriorityWaitTimeInSeconds { get; set; }
        public double averagePassPriorityWaitTimeInSeconds { get; set; }
        public int receivedPriorityCount { get; set; }
        public int passedPriorityCount { get; set; }
        public int respondedToPriorityCount { get; set; }
        public int spellsCastWithAutoPayCount { get; set; }
        public int spellsCastWithManualManaCount { get; set; }
        public int spellsCastWithMixedPayManaCount { get; set; }
        public string abilityUseByGrpId { get; set; }
        public string abilityCanceledByGrpId { get; set; }
        public string averageActionsByLocalPhaseStep { get; set; }
        public string averageActionsByOpponentPhaseStep { get; set; }
        public List<string> interactionCount { get; set; }
        public string playerId { get; set; }
    }

    public class LogInfoRequestRaw
    {
        public string id { get; set; }
        public string request { get; set; }
    }

    public class LogInfoRequestInnerRaw
    {
        public string jsonrpc { get; set; }
        public string method { get; set; }
        public Params @params { get; set; }
        public string id { get; set; }
    }
}
