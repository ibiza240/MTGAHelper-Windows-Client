using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    internal class SimpleZoneTracker : ZoneTrackerBase
    {
        protected readonly List<StateCard2> cards = new List<StateCard2>(7);

        public SimpleZoneTracker(OwnedZone forZone) : base(forZone) { }

        protected override int CardCount => cards.Count;

        public override IEnumerable<CardDrawInfo> GrpIdInfos => cards
            .Select(TryGetGroupId)
            .Where(i => i > 0)
            .GroupBy(i => i)
            .Select(g => new CardDrawInfo(g.Key, g.Count()));

        protected virtual int TryGetGroupId(StateCard2 card) => card.GrpId;

        public override void SetInstanceIds(IReadOnlyCollection<ITrackedCard> newCards)
        {
            cards.RemoveAll(c => newCards.All(i => i.InstId != c.InstId));

            var toAdd = newCards
                .Where(id => !cards.Exists(c => c.InstId == id.InstId));
            cards.AddRange(toAdd.Select(c => CreateStateCard(c.GrpId).UpdateInstanceId(c.InstId)));
        }

        public override void AddCards(IReadOnlyCollection<StateCard2> cardsToAdd)
        {
            cards.AddRange(cardsToAdd);
        }

        public override IReadOnlyCollection<StateCard2> TakeCards(IReadOnlyCollection<ITrackedCard> cardsToTake)
        {
            var ret = new List<StateCard2>(cardsToTake.Count);
            foreach (var cardToTake in cardsToTake)
            {
                var idx = cards.FindIndex(c => c.InstId == cardToTake.InstId);
                if (idx < 0)
                {
                    Log.Warning("(SimpleZoneTracker.TakeCards) cannot take card: {cardToTake}", cardToTake);
                    continue;
                }
                ret.Add(cards[idx]);
                cards.RemoveAt(idx);
            }

            return ret;
        }

        public override bool Clear()
        {
            cards.Clear();
            return true;
        }
    }
}