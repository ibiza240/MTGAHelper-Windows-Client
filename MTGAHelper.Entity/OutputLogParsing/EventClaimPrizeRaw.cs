using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public class DraftInfo
    {
        public string DraftId { get; set; }
    }

    public class WinLossGate
    {
        public int MaxWins { get; set; }
        public int MaxLosses { get; set; }
        public int MaxGames { get; set; }
        public int CurrentWins { get; set; }
        public int CurrentLosses { get; set; }
        public int CurrentGames { get; set; }
        public List<string> ProcessedMatchIds { get; set; }
    }

    public class ModuleInstanceData
    {
        public string HasPaidEntry { get; set; }
        public DraftInfo DraftInfo { get; set; }
        public bool DraftComplete { get; set; }
        public bool HasGranted { get; set; }
        public bool DeckSelected { get; set; }
        public WinLossGate WinLossGate { get; set; }
        public bool HasClaimedPrize { get; set; }
        public bool SealedPoolGenerated { get; set; }
    }

    public class MainDeck
    {
        public string id { get; set; }
        public int quantity { get; set; }
    }

    public class Sideboard
    {
        public string id { get; set; }
        public int quantity { get; set; }
    }

    public class CourseDeck
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string format { get; set; }
        public string resourceId { get; set; }
        public int deckTileId { get; set; }
        public List<MainDeck> mainDeck { get; set; }
        public List<Sideboard> sideboard { get; set; }
        public DateTime lastUpdated { get; set; }
        public bool lockedForUse { get; set; }
        public bool lockedForEdit { get; set; }
        public object cardBack { get; set; }
        public bool isValid { get; set; }
    }

    public class EventClaimPrizeRaw
    {
        public string Id { get; set; }
        public string InternalEventName { get; set; }
        public object PlayerId { get; set; }
        public ModuleInstanceData ModuleInstanceData { get; set; }
        public string CurrentEventState { get; set; }
        public string CurrentModule { get; set; }
        public List<int> CardPool { get; set; }
        public CourseDeck CourseDeck { get; set; }
        public List<object> PreviousOpponents { get; set; }
    }
}
