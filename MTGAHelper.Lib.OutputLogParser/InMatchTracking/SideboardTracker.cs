using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    [TrackerForZone(OwnedZone.OppSideboard)]
    [TrackerForZone(OwnedZone.MySideboard)]
    internal class SideboardTracker : ZoneTrackerBase
    {
        readonly List<int> grpIds = new List<int>(15);

        public SideboardTracker(OwnedZone forZone) : base(forZone) { }

        protected override int CardCount => grpIds.Count;

        public override IEnumerable<CardDrawInfo> GrpIdInfos => grpIds.GroupBy(i => i).Select(g => new CardDrawInfo(g.Key, g.Count()));

        public override void SetInstanceIds(IReadOnlyCollection<ITrackedCard> newCards) { }

        public override void AddCards(IReadOnlyCollection<StateCard2> cardsToAdd)
        {
            if (cardsToAdd == null)
                return;

            grpIds.AddRange(cardsToAdd.Select(c => c.GrpId));
        }

        public void SetGrpIds(IEnumerable<int> cardIds)
        {
            grpIds.Clear();
            if (cardIds != null)
                grpIds.AddRange(cardIds);
        }

        public override IReadOnlyCollection<StateCard2> TakeCards(IReadOnlyCollection<ITrackedCard> cardsToTake)
        {
            foreach (var c in cardsToTake) grpIds.Remove(c.GrpId);

            return cardsToTake.Select(c => CreateStateCard(c.GrpId).UpdateInstanceId(c.InstId)).ToArray();
        }

        public override bool Clear()
        {
            grpIds.Clear();
            return true;
        }
    }
}