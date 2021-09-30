using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.EventSetDeck;
using System;
using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.EventGetCourses
{
    public class EventGetCoursesResult : MtgaOutputLogPartResultBase<EventGetCoursesRaw>
    {
    }

    public partial class EventGetCoursesRaw
    {
        public ICollection<EventGetCourseRaw> Courses { get; set; }
    }

    public partial class EventGetCourseRaw : ICardPool
    {
        public Guid CourseId { get; set; }
        public string InternalEventName { get; set; }
        public long CurrentModule { get; set; }
        public string ModulePayload { get; set; }
        public CourseDeckSummary CourseDeckSummary { get; set; }
        public EventDeckRaw CourseDeck { get; set; }
        public List<int> CardPool { get; set; }
        public int CurrentWins { get; set; }
        public int CurrentLosses { get; set; }
    }
}