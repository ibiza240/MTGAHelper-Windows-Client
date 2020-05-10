using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    internal class InstanceIdEqualityComparer : IEqualityComparer<ITrackedCard>
    {
        public bool Equals(ITrackedCard c1, ITrackedCard c2)
        {
            return c1?.InstId == c2?.InstId;
        }

        public int GetHashCode(ITrackedCard card)
        {
            return card.InstId;
        }
    }
}