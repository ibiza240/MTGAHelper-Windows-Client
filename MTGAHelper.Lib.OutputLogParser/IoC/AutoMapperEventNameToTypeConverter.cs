using AutoMapper;
using MTGAHelper.Lib.EventsSchedule;

namespace MTGAHelper.Web.UI.Shared
{
    public class AutoMapperEventNameToTypeConverter : IValueConverter<string, string>
    {
        readonly IEventTypeCache eventsScheduleManager;

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
