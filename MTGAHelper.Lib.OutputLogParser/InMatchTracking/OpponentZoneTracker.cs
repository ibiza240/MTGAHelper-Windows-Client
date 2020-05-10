using System.Linq;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    internal class OpponentZoneTracker : SimpleZoneTracker
    {
        protected readonly OpponentCardTracker cardTracker;

        public OpponentZoneTracker(OwnedZone forZone, OpponentCardTracker cardTracker) : base(forZone)
        {
            this.cardTracker = cardTracker;
            cardTracker.RegisterZone(forZone, () => cards.Select(c => c.InstId));
        }

        protected override int TryGetGroupId(StateCard2 card)
        {
            return card.IsKnown ? card.GrpId : cardTracker.TryGetGroupIdFor(card.InstId);
        }
    }
}