using System;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public class MakeHumanDraftPickRaw
    {
        public Guid draftId { get; set; }
        public int cardId { get; set; }
        public int packNumber { get; set; }
        public int pickNumber { get; set; }
    }
}
