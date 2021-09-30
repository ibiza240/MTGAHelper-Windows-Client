using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.EventGetCourses;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class EventGetCoursesConverter : GenericConverter<EventGetCoursesResult, EventGetCoursesRaw>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Event_GetCourses";
    }
}