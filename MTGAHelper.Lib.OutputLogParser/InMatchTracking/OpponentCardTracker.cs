using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MTGAHelper.Utility;
using Serilog;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    internal class OpponentCardTracker
    {
        readonly InstanceIdEqualityComparer byInstId = new InstanceIdEqualityComparer();
        readonly HashSet<GameCardInZone> tempRevealed = new HashSet<GameCardInZone>(new InstanceIdEqualityComparer());
        IReadOnlyCollection<ITrackedCard> revealedCardIds = new ITrackedCard[0];

        /// <summary>key: InstId</summary>
        readonly Dictionary<int, GameCardInZone> knownCardsByInstId = new Dictionary<int, GameCardInZone>();
        /// <summary>key: GrpId</summary>
        readonly Dictionary<int, ShuffledGrpId> shuffledKnownCards = new Dictionary<int, ShuffledGrpId>();
        readonly List<IReadOnlyCollection<int>> cardsSeenPrevGames = new List<IReadOnlyCollection<int>>(2);
        int oppSeatId;

        readonly Dictionary<OwnedZone, Func<IEnumerable<int>>> instanceIdsByZone = new Dictionary<OwnedZone, Func<IEnumerable<int>>>();

        IEnumerable<GameCardInZone> CardsCurrentlyRevealed => tempRevealed.Intersect(revealedCardIds, byInstId).Cast<GameCardInZone>();

        IEnumerable<GameCardInZone> RevealedCardsInZones => knownCardsByInstId.Values.Where(c => c.Zone != OwnedZone.OppHand).Concat(CardsCurrentlyRevealed);

        public IEnumerable<CardDrawInfo> CardsSeen => RevealedCardsInZones
            .Select(c => c.GrpId)
            .Concat(shuffledKnownCards.Values
                .SelectMany(s => Enumerable.Repeat(s.GrpId, s.Count)))
            .GroupBy(grpId => grpId)
            .Select(g => new CardDrawInfo(g.Key, g.Count()));

        internal void RegisterZone(OwnedZone zone, Func<IEnumerable<int>> getInstanceIds)
        {
            if (instanceIdsByZone.ContainsKey(zone))
            {
                var ex = new ArgumentException($"zone {zone} already coupled!", nameof(zone));
                Log.Error(ex, $"tried to add zone {zone} that was already coupled!");
                return;
            }
            instanceIdsByZone.Add(zone, getInstanceIds);
        }

        public void SetOpponentSeatId(int id)
        {
            oppSeatId = id;
        }

        public void RevealedCardIdsAre(IReadOnlyCollection<ITrackedCard> cardIds)
        {
            revealedCardIds = cardIds;
        }

        public void CheckForRevealedCards(IEnumerable<GameCardInZone> newGameObjects)
        {
            var opponentRevealedCards = newGameObjects
                    .Where(nc => nc.IsKnown
                                 && nc.OwnerSeatId == oppSeatId
                                 && nc.Zone.ShouldTrackOpponentCards())
                    .ToLookup(o => o.GameObjectType);

            Log.Debug($"({nameof(CheckForRevealedCards)}) opponentRevealedCards: {{oppRevCards}}", opponentRevealedCards);

            foreach (var card in opponentRevealedCards[GameObjectType.GameObjectType_Card])
                RevealCard(card);

            foreach (var card in opponentRevealedCards[GameObjectType.GameObjectType_SplitCard])
                RevealCard(card);

            foreach (var card in opponentRevealedCards[GameObjectType.GameObjectType_RevealedCard])
                RevealTemp(card);
        }

        public void ProcessIdChanges(IEnumerable<IZoneAndInstanceIdChange> changes)
        {
            foreach (var idChange in changes)
            {
                if (idChange.GrpId != 0)
                    shuffledKnownCards.GetValueOrDefault(idChange.GrpId)?.HandleIdChange(idChange);
                else
                    foreach (var shuffled in shuffledKnownCards.Values)
                        shuffled.HandleIdChange(idChange);

                if (!knownCardsByInstId.TryGetValue(idChange.OldInstanceId, out var card))
                    continue;

                Log.Debug($"({nameof(ProcessIdChanges)}) moving {{idChange}}", idChange);
                knownCardsByInstId.Remove(idChange.OldInstanceId);
                if (idChange.DestZone.ShouldTrackOpponentCards())
                {
                    var newGrpId = idChange.GrpId > 0 ? idChange.GrpId : card.GrpId;
                    knownCardsByInstId.Add(idChange.NewInstanceId, card.MovedTo(idChange.NewInstanceId, idChange.DestZone, newGrpId));
                }

                if (idChange.GrpId == 0)
                    Log.Debug($"id change with idChange.GrpId == 0 (card grpId {card.GrpId})");
            }
        }

        public void ProcessShuffle(IReadOnlyCollection<int> oldIds, IReadOnlyCollection<int> newIds)
        {
            foreach (var shuffledKnownCard in shuffledKnownCards.Values)
                shuffledKnownCard.HandleNewShuffle(oldIds, newIds);

            foreach (var oldId in oldIds)
            {
                if (knownCardsByInstId.ContainsKey(oldId))
                {
                    var grpId = knownCardsByInstId[oldId].GrpId;
                    if (shuffledKnownCards.ContainsKey(grpId))
                        shuffledKnownCards[grpId].AddNewIds(newIds);
                    else
                        shuffledKnownCards.Add(grpId, new ShuffledGrpId(grpId, newIds));
                    knownCardsByInstId.Remove(oldId);
                }
            }
        }

        void RevealCard(GameCardInZone newCard)
        {
            if (knownCardsByInstId.ContainsKey(newCard.InstId))
                return;

            TryRemoveFromShuffled(newCard);

            knownCardsByInstId.Add(newCard.InstId, newCard);
            Log.Debug("added revealed card {newCard}", newCard);
        }

        void RevealTemp(GameCardInZone newCard)
        {
            // IMPROVE if a card is shuffled into library, then drawn and revealed in hand,
            // it will temporarily be counted double in opponent cards seen.
            // When the card is revealed "for real" (played or instanceId coupled to it),
            // the double counting will stop, giving the impression that a seen card got removed.
            // We could probably prevent this temporary ghost card from showing up,
            // but it's a lot of work for something that will rarely happen -> not worth it
            tempRevealed.Add(newCard);
            Log.Debug("added temp card {newCard}", newCard);
        }

        public void Reset(bool isBo3SoftReset)
        {
            tempRevealed.Clear();
            revealedCardIds = new ITrackedCard[0];
            knownCardsByInstId.Clear();
            shuffledKnownCards.Clear();

            if (isBo3SoftReset)
            {
                cardsSeenPrevGames.Add(knownCardsByInstId.Values.Select(c => c.GrpId).ToArray());
                return;
            }

            cardsSeenPrevGames.Clear();
            oppSeatId = 0;
        }

        public int TryGetGroupIdFor(int instId)
        {
            knownCardsByInstId.TryGetValue(instId, out var grpId);
            return grpId?.GrpId ?? 0;
        }

        bool TryRemoveFromShuffled(GameCardInZone newCard)
        {
            if (!shuffledKnownCards.TryGetValue(newCard.GrpId, out var shuffled))
                return false;

            var removed = shuffled.TryRemove(newCard.InstId);

            if (shuffled.Count == 0)
                shuffledKnownCards.Remove(newCard.GrpId);

            return removed;
        }

        class ShuffledGrpId
        {
            public ShuffledGrpId(int grpId, IEnumerable<int> newInstIds)
            {
                GrpId = grpId;
                Count = 1;
                NewInstIds = newInstIds.ToList();
            }

            public int GrpId { get; }
            public int Count { get; private set; }
            List<int> NewInstIds { get; }

            public void AddNewIds(IEnumerable<int> newIds)
            {
                var toAdd = newIds.Except(NewInstIds).ToArray();
                NewInstIds.AddRange(toAdd);
                Count += 1;
            }

            public void HandleIdChange(IZoneAndInstanceIdChange idChange)
            {
                var idx = NewInstIds.IndexOf(idChange.OldInstanceId);
                if (idx >= 0)
                {
                    NewInstIds[idx] = idChange.NewInstanceId;
                }
            }

            public void HandleNewShuffle(IReadOnlyCollection<int> oldIds, IReadOnlyCollection<int> newIds)
            {
                if (NewInstIds.Intersect(oldIds).Any())
                {
                    var newList = NewInstIds.Except(oldIds).Concat(newIds).ToArray();
                    NewInstIds.Clear();
                    NewInstIds.AddRange(newList);
                }
            }

            public bool TryRemove(int instId)
            {
                if (!NewInstIds.Remove(instId))
                    return false;

                Count -= 1;
                return true;

            }
        }
    }
}