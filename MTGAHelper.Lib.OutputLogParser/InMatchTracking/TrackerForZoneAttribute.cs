using System;
using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    internal sealed class TrackerForZoneAttribute : Attribute
    {
        public TrackerForZoneAttribute(params OwnedZone[] zones)
        {
            Zones = zones;
        }

        public IReadOnlyCollection<OwnedZone> Zones { get; }
    }
}
