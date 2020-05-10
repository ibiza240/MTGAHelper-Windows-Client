using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    internal class OppRevealedTracker : OpponentZoneTracker
    {
        public OppRevealedTracker(OwnedZone forZone, OpponentCardTracker cardTracker) : base(forZone, cardTracker) { }

        public override void SetInstanceIds(IReadOnlyCollection<ITrackedCard> newCards)
        {
            base.SetInstanceIds(newCards);
            cardTracker.RevealedCardIdsAre(newCards);
        }
    }
}