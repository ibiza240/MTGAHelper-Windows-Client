using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.EventSetDeck;
using System;
using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.CompleteDraft
{
    public partial class CompleteDraftRaw : ICardPool
    {
        public Guid CourseId { get; set; }
        public string InternalEventName { get; set; }
        public int CurrentModule { get; set; }
        public string ModulePayload { get; set; }
        public CourseDeckSummary CourseDeckSummary { get; set; }
        public dynamic CourseDeck { get; set; }
        public int CurrentWins { get; set; }
        public int CurrentLosses { get; set; }
        public List<int> CardPool { get; set; }
        public dynamic JumpStart { get; set; }
        public Guid DraftId { get; set; }
    }
}