using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    internal abstract class ZoneTrackerBase : IZoneTracker
    {
        protected ZoneTrackerBase(OwnedZone forZone)
        {
            ForZone = forZone;
        }

        protected OwnedZone ForZone { get; }
        protected abstract int CardCount { get; }
        public abstract IEnumerable<CardDrawInfo> GrpIdInfos { get; }
        public abstract void SetInstanceIds(IReadOnlyCollection<ITrackedCard> newCards);
        public abstract void AddCards(IReadOnlyCollection<StateCard2> cardsToAdd);
        public abstract IReadOnlyCollection<StateCard2> TakeCards(IReadOnlyCollection<ITrackedCard> cardsToTake);
        public abstract bool Clear();

        protected bool IsTrackingOpponentZone => ForZone.IsOpponentZone();
        protected StateCard2 CreateStateCard(int grpId)
        {
            return new StateCard2(grpId, ForZone);
        }

        public override string ToString()
        {
            return $"{ForZone} ({CardCount} cards)";
        }
    }
}