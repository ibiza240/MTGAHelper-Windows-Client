using System;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.DraftNotify
{
    public class DraftNotifyRaw
    {
        public Guid draftId { get; set; }
        public int SelfPick { get; set; }
        public int SelfPack { get; set; }
        public string PackCards { get; set; }
    }
}