using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetActiveEventsV2;
using System;
using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetActiveEventsV3
{
    public class GetActiveEventsV3Raw
    {
        public string InternalEventName { get; set; }
        public string EventState { get; set; }
        public string EventType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime LockedTime { get; set; }
        public DateTime ClosedTime { get; set; }
        public IList<string> Flags { get; set; }
        public object PastEntries { get; set; }
        public IList<EntryFee> EntryFees { get; set; }
        public EventUXInfo EventUXInfo { get; set; }
    }

    public class EntryFee
    {
        public string CurrencyType { get; set; }
        public int Quantity { get; set; }
        public int? MaxUses { get; set; }
        public string ReferenceId { get; set; }
    }

    public class EventUXInfo
    {
        public string PublicEventName { get; set; }
        public int DisplayPriority { get; set; }
        public Parameters Parameters { get; set; }
        public string Group { get; set; }
        public string EventBladeBehaviour { get; set; }
        public string DeckSelectFormat { get; set; }
        public object Prizes { get; set; }
        public object EventComponentData { get; set; }
    }
}