using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using MTGAHelper.Lib.OutputLogParser.Exceptions;
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
        public LibraryTracker(OwnedZone forZone) : base(forZone)
        {
        }

        private readonly List<List<int>> cardIds = new List<List<int>>(1);

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

        private IReadOnlyDictionary<int, float> GetDrawChancesTopCard()
        {
            if (cards == null || cards.Count <= drawCardIdx)
                return new Dictionary<int, float>(0);

            return cards[drawCardIdx].GetDrawChances();
        }

        /// <summary>Ordered collection of the instanceIds in the library</summary>
        private List<LibraryCard> cards;

        private ReadOnlyCollection<int> originalGrpIds;

        /// <summary>usually 0, unless viewing the top card(s) of the library</summary>
        private int drawCardIdx;

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
            // check if whole library shuffled (easier case)
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
                drawCardIdx = 0;

                Log.Debug("Shuffling complete.{nl}New cards    : {cards}", Environment.NewLine, cards);
                return;
            }

            var intersect = oldIds.Intersect(cards.Select(c => c.InstId)).ToArray();
            if (!intersect.Any())
                return;

            drawCardIdx = 0;
            Log.Debug("Shuffle cards in lib {fz}?", ForZone);

            if (intersect.Length < oldIds.Count)
            {
                Log.Warning(
                    "(ShuffleCards) WEIRDLY, some cards overlap, but not all. Intersection: {intersect}; oldIds: {oldIds}",
                    cards.Where(c => oldIds.Contains(c.InstId)), oldIds);
                Debugger.Break();
                return;
            }

            var oldCards = oldIds.Select(id => cards.First(c => c.InstId == id)).ToArray();
            if (oldCards.Any(c => !c.IsKnown))
            {
                // Lukka apparently does not reveal cards he sees... so we need a workaround
                ShuffleCardsWithUnknown(oldCards, newIds);
                return;
            }

            var grpIds = oldCards.Select(c => c.GrpId).ToList();
            cardIds.Add(grpIds);
            foreach (var c in oldCards)
            {
                c.SetRevealed(c.GrpId);
            }
            CleanCardIds();

            cards.RemoveAll(c => oldIds.Contains(c.InstId));
            cards.AddRange(newIds.Select(i => new LibraryCard(i, grpIds)));

            Log.Debug("Shuffling complete.{nl}New cards    : {cards}", Environment.NewLine, cards);
        }

        private void ShuffleCardsWithUnknown(IReadOnlyCollection<LibraryCard> oldCards, IReadOnlyCollection<int> newIds)
        {
            // known issue: after this, some cards that should be known to be in the bottom half of de deck might still show up in de draw%...
            // not as accurate as maybe possible, but it's the best we can do with the current tracking method

            var (knownCards, unknownCards) = oldCards.SplitBy(c => c.IsKnown);
            var distinctGrpIdGroups = unknownCards.Select(c => c.PossibleGrpIds).Distinct().ToArray();
            cards.RemoveAll(c => oldCards.Any(o => o.InstId == c.InstId));

            List<int> grpIds;
            if (!distinctGrpIdGroups.Any())
                throw new InvalidOperationException("ShuffleCardsWithUnknown: distinctGrpIdGroups empty!");

            if (distinctGrpIdGroups.Length == 1)
            {
                grpIds = distinctGrpIdGroups[0];
            }
            else
            {
                grpIds = distinctGrpIdGroups.SelectMany(i => i).ToList();

                var cardsWithOverlappingGrpIdGroups = cards
                    .Select((c, i) => (c, i))
                    .Where(x => distinctGrpIdGroups.Contains(x.c.PossibleGrpIds))
                    .Select(x => x.i)
                    .ToArray();
                foreach (var i in cardsWithOverlappingGrpIdGroups)
                {
                    cards[i] = new LibraryCard(cards[i].InstId, grpIds);
                }

                foreach (var distinctGrpIdGroup in distinctGrpIdGroups)
                {
                    cardIds.Remove(distinctGrpIdGroup);
                }
                cardIds.Add(grpIds);
            }

            foreach (var knownCard in knownCards)
            {
                grpIds.Add(knownCard.GrpId);
                knownCard.SetRevealed(knownCard.GrpId);
            }
            CleanCardIds();

            var newCards = newIds.Select(i => new LibraryCard(i, grpIds));
            cards.AddRange(newCards);
        }

        public bool ScryComplete(IReadOnlyCollection<int> topIds, IReadOnlyCollection<int> bottomIds)
        {
            // Scrying triggers SetInstIdsAboutToMove(), reset when Scry done.
            drawCardIdx = 0;

            var found = false;

            if (topIds != null)
                found |= MoveToTop(topIds);

            if (bottomIds != null)
                found |= MoveToBottom(bottomIds);

            return found;
        }

        private bool MoveToTop(IReadOnlyCollection<int> topIds) => MoveCards(topIds, c => cards.InsertRange(0, c));

        private bool MoveToBottom(IReadOnlyCollection<int> bottomIds) => MoveCards(bottomIds, cards.AddRange);

        private bool MoveCards(IReadOnlyCollection<int> ids, Action<IEnumerable<LibraryCard>> insertAction)
        {
            var toMove = ids.Select(i => cards.Find(c => c.InstId == i)).ToArray();
            if (toMove.All(c => c == null))
                return false;

            if (toMove.Any(c => c == null))
                Log.Warning("could not find cards {ids} in library", ids.Where(i => cards.All(c => c.InstId != i)));

            cards.RemoveAll(c => ids.Contains(c.InstId));
            insertAction(toMove.Where(c => c != null));
            return true;
        }

        public override void SetInstanceIds(IReadOnlyCollection<ITrackedCard> newCards)
        {
            if (newCards == null)
                throw new ArgumentNullException(nameof(newCards));

            //if (cards == null)
            //    Debugger.Break();

            drawCardIdx = 0;

            if (cards.Count != newCards.Count)
            {
                Log.Warning("(LibraryTracker.SetInstanceIds) counts differ: {cardsCount} != {newCardsCount}", cards.Count, newCards.Count);
                try
                {
                    TryGetCountsEqual(newCards);
                }
                catch (InvalidTrackerOperationException)
                {
                    // IMPROVE: add method to request a reverse engineer of the grpIds in library (by inspecting other trackers' cards)
                    // and keep tracking library (albeit with reduced accuracy, probably)
                    //TryReverseEngineerGrpIdsInLib();
                    throw;
                }
            }

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

        private void TryGetCountsEqual(IReadOnlyCollection<ITrackedCard> newCards)
        {
            // handle added cards
            var addedCards = newCards.Where(nc => cards.All(c => c.InstId != nc.InstId)).ToArray();
            if (addedCards.Any(c => !c.IsKnown))
                throw new InvalidTrackerOperationException("unknown cards added to library");

            cards.AddRange(CreateKnownCards(addedCards));

            // handle removed cards
            var removedCards = cards.Where(c => newCards.All(nc => nc.InstId != c.InstId)).ToArray();
            if (removedCards.Any(c => !c.IsKnown))
                throw new InvalidTrackerOperationException("unknown cards removed from library");

            TakeCardsInternal(removedCards);
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

        private void RevealCard(ITrackedCard card)
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
            drawCardIdx = 0;

            TakeCardsInternal(cardsToTake);

            // IMPROVE: maybe track StateCards that remain known (e.g. cards put on top by Mystic Sanctuary or Witch's Cottage)
            return cardsToTake.Select(c => CreateStateCard(c.GrpId).UpdateInstanceId(c.InstId)).ToArray();
        }

        private void TakeCardsInternal(IReadOnlyCollection<ITrackedCard> cardsToTake)
        {
            foreach (var toTake in cardsToTake)
            {
                Debug.Assert(toTake.IsKnown || IsTrackingOpponentZone);

                var idx = cards.FindIndex(c => c.InstId == toTake.InstId);
                if (idx < 0)
                {
                    var ex = new InvalidTrackerOperationException($"[Message summarized?] Failed to match instanceId {toTake.InstId}!");
                    Log.Error(ex, "(LibraryTracker.TakeCards) card toTake {toTake}{nl}not in cards : {cards}",
                        toTake, Environment.NewLine, cards);
                    throw ex;
                }

                cards[idx].SetRevealed(toTake.GrpId);
                cards.RemoveAt(idx);
            }

            CleanCardIds();
        }

        public override bool Clear()
        {
            Log.Debug("(LibraryTracker.Clear) clearing...");

            cardIds.Clear();
            cards = null;
            originalGrpIds = null;
            drawCardIdx = 0;
            return true;
        }

        /// <summary>Remove empty collections to prevent clogging cardIds</summary>
        private void CleanCardIds()
        {
            cardIds.RemoveAll(col => col.Count <= 0);
        }

        /// <summary>
        /// returns true if cards were already ordered or if card reordering was successful,
        /// returns false if set of instanceIds differ
        /// </summary>
        private bool TryReOrderCards(IReadOnlyCollection<ITrackedCard> newCards)
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

        private IEnumerable<LibraryCard> CreateKnownCards(IEnumerable<ITrackedCard> cardsToCreate)
        {
            var ret = cardsToCreate.Select(CreateKnownCard).ToArray();
            return ret;
        }

        private LibraryCard CreateKnownCard(ITrackedCard card)
        {
            Debug.Assert(card.IsKnown);

            var possibleGrpIds = new List<int>(1) { card.GrpId };
            cardIds.Add(possibleGrpIds);
            return new LibraryCard(card.InstId, possibleGrpIds);
        }

        private class LibraryCard : CardIds
        {
            private bool isKnown;
            public override int GrpId => IsKnown ? PossibleGrpIds.First() : 0;
            internal List<int> PossibleGrpIds { get; }

            private object lockPossibleGrpIds = new object();

            public override bool IsKnown => isKnown;

            public LibraryCard(int instId, List<int> possibleGrpIds) : base(instId)
            {
                lock (lockPossibleGrpIds)
                {
                    PossibleGrpIds = possibleGrpIds ?? throw new ArgumentNullException(nameof(possibleGrpIds));
                    isKnown = PossibleGrpIds.IsRepeatedUniqueValue();
                }
            }

            public void SetRevealed(int grpId)
            {
                lock (lockPossibleGrpIds)
                {
                    if (PossibleGrpIds.Remove(grpId) || PossibleGrpIds.Remove(0))
                    {
                        isKnown = PossibleGrpIds.IsRepeatedUniqueValue();
                        return;
                    }

                    if (!IsKnown)
                        Log.Warning("(LibraryCard.SetRevealed) couldn't remove {grpId} from {possibleGrpIds}", grpId, PossibleGrpIds);
                }
            }

            public IReadOnlyDictionary<int, float> GetDrawChances()
            {
                lock (lockPossibleGrpIds)
                {
                    if (IsKnown)
                        return new Dictionary<int, float> { { GrpId, 1f } };

                    return PossibleGrpIds
                        .Distinct()
                        .Select(grpId => (grpId, chance: (float)PossibleGrpIds.Count(i => i == grpId) / PossibleGrpIds.Count))
                        .ToDictionary(x => x.grpId, x => x.chance);
                }
            }
        }

        public void SetInstIdsAboutToMove(IReadOnlyCollection<int> instanceIds)
        {
            if (cards.Take(instanceIds.Count).Select(c => c.InstId).Except(instanceIds).Any())
                return;
            drawCardIdx = instanceIds.Count;
        }
    }
}