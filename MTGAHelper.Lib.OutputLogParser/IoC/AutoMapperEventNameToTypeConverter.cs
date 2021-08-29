using AutoMapper;
using MTGAHelper.Lib.OutputLogParser.EventsSchedule;

namespace MTGAHelper.Lib.OutputLogParser.IoC
{
    public class AutoMapperEventNameToTypeConverter : IValueConverter<string, string>
    {
        private readonly IEventTypeCache eventsScheduleManager;

        public AutoMapperEventNameToTypeConverter(IEventTypeCache eventsScheduleManager)
        {
            this.eventsScheduleManager = eventsScheduleManager;
        }

        public string Convert(string sourceMember, ResolutionContext context)
        {
            return eventsScheduleManager.GetEventType(sourceMember);
        }
    }
}