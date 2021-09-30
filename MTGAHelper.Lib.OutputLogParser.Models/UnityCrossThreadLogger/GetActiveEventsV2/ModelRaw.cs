using System;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetActiveEventsV2
{
    public class GetActiveEventsV2Raw
    {
        public string PublicEventName { get; set; }
        public string InternalEventName { get; set; }
        public string EventState { get; set; }
        public string EventType { get; set; }
        public ModuleGlobalData ModuleGlobalData { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime LockedTime { get; set; }
        public DateTime ClosedTime { get; set; }
        public Parameters Parameters { get; set; }
        public string Group { get; set; }
        public object PastEntries { get; set; }
        public int DisplayPriority { get; set; }
        public bool IsArenaPlayModeEvent { get; set; }
        public object Emblems { get; set; }
        public bool UsePlayerCourse { get; set; }
    }

    public class ModuleGlobalData
    {
        public string DeckSelect { get; set; }
    }

    public class Parameters
    {
    }
}