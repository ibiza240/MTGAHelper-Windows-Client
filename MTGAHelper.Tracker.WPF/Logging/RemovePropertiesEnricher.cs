using Serilog.Core;
using Serilog.Events;

namespace MTGAHelper.Tracker.WPF.Logging
{
    public class RemovePropertiesEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent le, ILogEventPropertyFactory lepf)
        {
            le.RemovePropertyIfPresent("outputLogError");
        }
    }
}
