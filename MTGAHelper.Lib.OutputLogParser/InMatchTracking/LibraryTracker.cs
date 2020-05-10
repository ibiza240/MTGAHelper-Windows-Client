using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using MTGAHelper.Utility;
using Serilog;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    /// <summary>
    /// Used to track library.
    /// Library is different from other zones in that it is hidden and its instanceIds are ordered
    /// </summary>
    [TrackerForZone(OwnedZone.OppLibrary)]
    [TrackerForZone(OwnedZone.MyLibrary)]
    internal class LibraryTracker : ZoneTrackerBase
    {
        public LibraryTracker(OwnedZone forZone) : base(forZone) { }

        readonly List<List<int>> cardIds = new List<List<int>>(1);

        protected override int CardCount => cards?.Count ?? 0;

        public override IEnumerable<CardDrawInfo> GrpIdInfos
        {
            get
            {
                var drawChances = GetDrawChancesTopCard();
                return cardIds
                    .SelectMany(g => g)
                    .GroupBy(i => i)
                    .Select(g => new CardDrawInfo(g.Key, g.Count(), drawChances.GetValueOrDefault(g.Key)));
            }
        }

        IReadOnlyDictionary<int, float> GetDrawChancesTopCard()
        {
            if (cards == null || cards.Count <= topCardIdx)
                return new Dictionary<int, float>(0);

            return cards[topCardIdx].GetDrawChances();
        }

        /// <summary>Ordered collection of the instanceIds in the library</summary>
        List<LibraryCard> cards;

        ReadOnlyCollection<int> originalGrpIds;
        int topCardIdx;


        public void ResetWithGrpIds(IEnumerable<int> grpIds)
        {
            Clear();
            originalGrpIds = grpIds.ToList().AsReadOnly();
            cardIds.Add(originalGrpIds.ToList());
        }

        public IEnumerable<StateCard2> DrawOrMulligan(IReadOnlyCollection<int> instanceIds, IReadOnlyCollection<ITrackedCard> toHand)
        {
            if (originalGrpIds == null || originalGrpIds.Count == 0)
                originalGrpIds = Enumerable.Repeat(0, instanceIds.Count + toHand.Count).ToList().AsReadOnly();

            cardIds.Clear();
            cardIds.Add(originalGrpIds.ToList());

            foreach (var c in toHand)
            {
                cardIds[0].Remove(c.GrpId);
            }

            cards = instanceIds
                .Select(i => new LibraryCard(i, cardIds[0]))
                .ToList();

            Log.Debug("draw or mulligan processed.{nl}Tracker.cards: {cards}", Environment.NewLine, cards);

            return toHand.Select(ids => CreateStateCard(ids.GrpId).UpdateInstanceId(ids.InstId));
        }

        public void ShuffleCards(IReadOnlyCollection<int> oldIds, IReadOnlyCollection<int> newIds)
        {
            if (cards.Count == oldIds.Count)
            {
                Log.Debug("Shuffle lib {fz}?", ForZone);
                if (!cards.Select(c => c.InstId).SequenceEqual(oldIds))
                    return;

                Log.Debug("Shuffling!");

                var grpIdsInLib = cardIds.SelectMany(id => id).ToList();
                cardIds.Clear();
                cardIds.Add(grpIdsInLib);
                cards = newIds.Select(i => new LibraryCard(i, grpIdsInLib)).ToList();
                topCardIdx = 0;

                Log.Debug("Shuffling complete.{nl}New cards    : {cards}", Environment.NewLine, cards);
            }
            else if (oldIds.Intersect(cards.Select(c => c.InstId)).Any())
            {
                Log.Warning($"(ShuffleCards) cards.Count {cards.Count} != {oldIds.Count} oldIds.Count but overlapping cards detected: {{intersect}}",
                    cards.Where(c => oldIds.Contains(c.InstId)));
                Debugger.Break();
            }
        }

        public override void SetInstanceIds(IReadOnlyCollection<ITrackedCard> newCards)
        {
            if (newCards == null)
                throw new ArgumentNullException(nameof(newCards));

            if (cards == null)
                Debugger.Break();

            if (cards.Count != newCards.Count)
            {
                Log.Error("(LibraryTracker.SetInstanceIds) counts differ: {cardsCount} != {newCardsCount}", cards.Count, newCards.Count);
                throw new ArgumentOutOfRangeException(nameof(newCards), newCards, null);
            }

            topCardIdx = 0;
            if (TryReOrderCards(newCards) == false)
            {
                if (cardIds.Count <= 1)
                {
                    Log.Warning("no known cards; reset of cards possible");
                    var possibleCardIds = cardIds.FirstOrDefault();
                    cards = newCards.Select(n => new LibraryCard(n.InstId, possibleCardIds)).ToList();
                }
                else
                {
                    var (knownRemovals, unknownRemovals) = cards
                        .Where(i => newCards.All(n => n.InstId != i.InstId))
                        .SplitBy(p => p.IsKnown);

                    var (knownAdditions, unknownAdditions) = newCards
                        .Where(n => cards.All(c => c.InstId != n.InstId))
                        .SplitBy(i => i.GrpId != 0);

                    Log.Warning("knownRemovals: {knownRemovals}}", knownRemovals);
                    Log.Warning("unknownRemovals: {unknownRemovals}}", unknownRemovals);
                    Log.Warning("knownAdditions: {knownAdditions}}", knownAdditions);
                    Log.Warning("unknownAdditions: {unknownAdditions}}", unknownAdditions);
                }
            }

            RevealCards(newCards);
        }

        public void RevealCards(IEnumerable<ITrackedCard> revealedCards)
        {
            var reallyRevealed = revealedCards.Where(c => c.IsKnown).ToArray();
            if (reallyRevealed.Length == 0)
                return;

            Log.Debug("(LibraryTracker.RevealCards) revealing {reallyRevealed}", reallyRevealed);

            foreach (var revealedCard in reallyRevealed)
            {
                RevealCard(revealedCard);
            }
            CleanCardIds();
        }

        void RevealCard(ITrackedCard card)
        {
            Debug.Assert(card.GrpId > 0);

            var idx = cards.FindIndex(c => c.InstId == card.InstId);
            if (idx < 0 || cards[idx].GrpId == card.GrpId)
                return;

            cards[idx].SetRevealed(card.GrpId);
            cards[idx] = CreateKnownCard(card);
        }

        public override void AddCards(IReadOnlyCollection<StateCard2> cardsToAdd)
        {
            Log.Debug("(LibraryTracker.AddCards) adding {cards}", cardsToAdd);
            cards.AddRange(CreateKnownCards(cardsToAdd.Select(c => new CardIds(c.InstId, c.GrpId))));
        }

        public override IReadOnlyCollection<StateCard2> TakeCards(IReadOnlyCollection<ITrackedCard> cardsToTake)
        {
            Log.Debug("(LibraryTracker.TakeCards) taking {cards}", cardsToTake);
            topCardIdx = 0;

            foreach (var toTake in cardsToTake)
            {
                Debug.Assert(toTake.IsKnown || IsTrackingOpponentZone);

                var idx = cards.FindIndex(c => c.InstId == toTake.InstId);
                if (idx < 0)
                {
                    var ex = new ArgumentException($"[Message summarized?] Failed to match instanceId {toTake.InstId}!", nameof(cardsToTake));
                    Log.Error(ex, "(LibraryTracker.TakeCards) card toTake {toTake}{nl}not in cards : {cards}",
                        toTake, Environment.NewLine, cards);
                    Debugger.Break();
                    throw ex;
                }

                cards[idx].SetRevealed(toTake.GrpId);
                cards.RemoveAt(idx);
            }

            CleanCardIds();

            // IMPROVE: maybe track StateCards that remain known (e.g. cards put on top by Mystic Sanctuary or Witch's Cottage)
            return cardsToTake.Select(c => CreateStateCard(c.GrpId).UpdateInstanceId(c.InstId)).ToArray();
        }

        public override bool Clear()
        {
            Log.Debug("(LibraryTracker.Clear) clearing...");

            cardIds.Clear();
            cards = null;
            originalGrpIds = null;
            topCardIdx = 0;
            return true;
        }

        /// <summary>Remove empty collections to prevent clogging cardIds</summary>
        void CleanCardIds()
        {
            cardIds.RemoveAll(col => col.Count <= 0);
        }

        /// <summary>
        /// returns true if cards were already ordered or if card reordering was successful,
        /// returns false if set of instanceIds differ
        /// </summary>
        bool TryReOrderCards(IReadOnlyCollection<ITrackedCard> newCards)
        {
            if (newCards.Count != cards?.Count)
                return false;

            if (cards.Select(c => c.InstId).SequenceEqual(newCards.Select(c => c.InstId)))
            {
                Log.Debug("card lists were identical, nothing to reorder");
                return true;
            }

            Log.Debug("reordering cards. cards{nl}beforesorting: {cards}{nl}newCards     : {newCards}",
                Environment.NewLine, cards, Environment.NewLine, newCards);

            var positionDict = newCards.Select((id, idx) => (id.InstId, idx)).ToDictionary(t => t.InstId, t => t.idx);
            try
            {
                cards.Sort((left, right) => positionDict[left.InstId].CompareTo(positionDict[right.InstId]));
                Log.Debug("reorder complete");
                return true;
            }
            catch (InvalidOperationException)
            // List<>.Sort() throws InvalidOperationException if comparer misbehaves; ours throws if unmatched instId
            {
                Log.Debug("cards different from newCards. cards{nl}after sorting: {cards}", Environment.NewLine, cards);
                return false;
            }
        }

        IEnumerable<LibraryCard> CreateKnownCards(IEnumerable<ITrackedCard> cardsToCreate)
        {
            var ret = cardsToCreate.Select(CreateKnownCard).ToArray();
            return ret;
        }

        LibraryCard CreateKnownCard(ITrackedCard card)
        {
            Debug.Assert(card.IsKnown);

            var possibleGrpIds = new List<int>(1) { card.GrpId };
            cardIds.Add(possibleGrpIds);
            return new LibraryCard(card.InstId, possibleGrpIds);
        }

        class LibraryCard : CardIds
        {
            bool isKnown;
            public override int GrpId => IsKnown ? PossibleGrpIds.First() : 0;
            ICollection<int> PossibleGrpIds { get; }

            public override bool IsKnown => isKnown;

            public LibraryCard(int instId, ICollection<int> possibleGrpIds) : base(instId)
            {
                PossibleGrpIds = possibleGrpIds ?? throw new ArgumentNullException(nameof(possibleGrpIds));
                isKnown = PossibleGrpIds.IsRepeatedUniqueValue();
            }

            public void SetRevealed(int grpId)
            {
                if (PossibleGrpIds.Remove(grpId) || PossibleGrpIds.Remove(0))
                {
                    isKnown = PossibleGrpIds.IsRepeatedUniqueValue();
                    return;
                }

                if (!IsKnown)
                    Log.Warning("(LibraryCard.SetRevealed) couldn't remove {grpId} from {possibleGrpIds}", grpId, PossibleGrpIds);
            }

            public IReadOnlyDictionary<int, float> GetDrawChances()
            {
                if (IsKnown)
                    return new Dictionary<int, float> { { GrpId, 1f } };

                return PossibleGrpIds
                    .Distinct()
                    .Select(grpId => (grpId, chance: (float)PossibleGrpIds.Count(i => i == grpId) / PossibleGrpIds.Count))
                    .ToDictionary(x => x.grpId, x => x.chance);
            }
        }

        public void SetInstIdsAboutToMove(IReadOnlyCollection<int> instanceIds)
        {
            if (cards.Take(instanceIds.Count).Select(c => c.InstId).Except(instanceIds).Any())
                return;
            topCardIdx = instanceIds.Count;
        }
    }
}
