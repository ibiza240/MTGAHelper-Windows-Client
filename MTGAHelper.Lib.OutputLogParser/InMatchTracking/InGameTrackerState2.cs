using System;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.GameStateMessage;
using Newtonsoft.Json;
using Serilog;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    public class InGameTrackerState2 : IInGameState
    {
        public int MySeatId { get; private set; }
        public int OpponentSeatId { get; private set; }
        public int PriorityPlayer { get; internal set; }
        public string OpponentScreenName { get; set; }


        public Dictionary<int, Player> Players { get; } = new Dictionary<int, Player>(2);

        readonly IReadOnlyDictionary<OwnedZone, IZoneTracker> cardsInZones;
        readonly OpponentCardTracker oppCardTracker;

        static IReadOnlyDictionary<OwnedZone, IZoneTracker> CreateZoneTrackers(OpponentCardTracker oppCardTracker)
        {
            return EnumExtensions.EveryZone().ToDictionary(z => z, zone => GetZoneTracker(zone, oppCardTracker));
        }

        static IZoneTracker GetZoneTracker(OwnedZone zone, OpponentCardTracker oppCardTracker)
        {
            switch (zone)
            {
                case OwnedZone.MyLibrary:
                    return new LibraryTracker(zone);
                case OwnedZone.MySideboard:
                case OwnedZone.OppSideboard:
                    return new SideboardTracker(zone);
                case OwnedZone.Unknown:
                case OwnedZone.Battlefield:
                case OwnedZone.Exile:
                case OwnedZone.Stack:
                case OwnedZone.Limbo:
                case OwnedZone.OppGraveyard:
                case OwnedZone.OppCommand:
                case OwnedZone.MyGraveyard:
                case OwnedZone.MyHand:
                case OwnedZone.MyCommand:
                case OwnedZone.Pending:
                case OwnedZone.MyRevealed:
                case OwnedZone.MyPhasedOut:
                    return new SimpleZoneTracker(zone);
                case OwnedZone.OppLibrary:
                case OwnedZone.OppHand:
                case OwnedZone.OppPhasedOut:
                    return new OpponentZoneTracker(zone, oppCardTracker);
                case OwnedZone.OppRevealed:
                    return new OppRevealedTracker(zone, oppCardTracker);
                default:
                    throw new ArgumentOutOfRangeException(nameof(zone), zone, null);
            }
        }

        LibraryTracker MyLibraryTracker => (LibraryTracker)cardsInZones[OwnedZone.MyLibrary];

        public IReadOnlyCollection<CardDrawInfo> MyLibrary => MyLibraryTracker.GrpIdInfos.ToArray();
        public IReadOnlyCollection<CardDrawInfo> MySideboard => cardsInZones[OwnedZone.MySideboard].GrpIdInfos.ToArray();
        public IReadOnlyCollection<CardDrawInfo> OpponentCardsSeen => oppCardTracker.CardsSeen.ToArray();

        public bool IsReset { get; internal set; }
        public bool IsSideboarding { get; internal set; }
        public int MyMulliganCount { get; internal set; }

        IEnumerable<string> TrackedZones => cardsInZones.Values.Select(t => t.ToString());

        public InGameTrackerState2()
        {
            oppCardTracker = new OpponentCardTracker();
            cardsInZones = CreateZoneTrackers(oppCardTracker);
            Reset();
        }

        public void Reset(bool isBo3SoftReset = false)
        {
            IsReset = true;

            PriorityPlayer = 0;
            MyMulliganCount = -1;
            foreach (var zoneTracker in cardsInZones.Values)
            {
                zoneTracker.Clear();
            }

            oppCardTracker.Reset(isBo3SoftReset);

            if (isBo3SoftReset)
                return;

            MySeatId = 0;
            OpponentSeatId = 0;
            OpponentScreenName = null;
            Players.Clear();
        }

        public void SetSeatIds(int mySeat, int oppSeat = 0)
        {
            MySeatId = mySeat;
            OpponentSeatId = oppSeat != 0 ? oppSeat : mySeat == 1 ? 2 : 1;
            oppCardTracker.SetOpponentSeatId(OpponentSeatId);
        }

        public void SetLibraryGrpIds(IEnumerable<int> grpIds)
        {
            MyLibraryTracker.ResetWithGrpIds(grpIds);
        }

        public void SetSideboardGrpIds(IEnumerable<int> grpIds)
        {
            ((SideboardTracker)cardsInZones[OwnedZone.MySideboard]).SetGrpIds(grpIds);
        }

        public void HandleMoves(IReadOnlyCollection<IZoneAndInstanceIdChange> zoneTransfers)
        {
            if (zoneTransfers == null || zoneTransfers.Count == 0)
                return;

            foreach (var zoneTransferInfo in zoneTransfers.OfType<ZoneTransferInfo2>())
            {
                if (zoneTransferInfo.Category == "CastSpell")
                    Log.Information("Cast spell: {zoneTransferInfo}", zoneTransferInfo);
                else if (zoneTransferInfo.Category == "Resolve")
                    Log.Information("Spell resolved: {zoneTransferInfo}", zoneTransferInfo);
            }

            foreach (var transfer in zoneTransfers.GroupBy(zt => new { zt.SrcZone, zt.DestZone }))
            {
                // GroupBy returns a Lookup<>, which encapsulates Grouping[], Grouping is an internal class that implements IList<>
                // a bit hack-y to cast to IList<> here as the implementation could conceivably change... probably won't happen though, so this is fine.
                HandleMoveFromTo(transfer.Key.SrcZone, transfer.Key.DestZone, (IList<IZoneAndInstanceIdChange>)transfer);
            }
        }

        void HandleMoveFromTo(OwnedZone srcZone, OwnedZone destZone, ICollection<IZoneAndInstanceIdChange> moves)
        {
            try
            {
                var cards = cardsInZones[srcZone].TakeCards(moves
                    .Select(m => new CardIds(m.OldInstanceId, m.GrpId))
                    .ToArray());

                var tempArray = moves.Join(cards,
                    m => m.OldInstanceId,
                    c => c.InstId,
                    (change, card) => card
                        .UpdateInstanceId(change.NewInstanceId)
                        .MoveToZone(change.DestZone, change.SrcZone))
                    .ToArray(); // Force evaluation

                if (tempArray.Length != moves.Count)
                {
                    Log.Warning("(InGameTrackerState2.HandleMoveFromTo) invalid move(s) detected. All moves: {moves}{nl}State: {this}",
                        moves, Environment.NewLine, this);
                }

                cardsInZones[destZone].AddCards(cards);

                oppCardTracker.ProcessIdChanges(moves);
            }
            catch (Exception e)
            {
                Log.Warning(e, "(HandleMoveTo) exception swallowed");
            }
        }

        public void HandleShuffles(
            IEnumerable<(IReadOnlyCollection<int> oldIds, IReadOnlyCollection<int> newIds)> shuffles)
        {
            if (shuffles == null)
                return;

            foreach (var (oldIds, newIds) in shuffles)
            {
                MyLibraryTracker.ShuffleCards(oldIds, newIds);
                oppCardTracker.ProcessShuffle(oldIds, newIds);
            }
        }

        public void SetInstIdsAboutToMove(IReadOnlyCollection<int> instanceIds)
        {
            MyLibraryTracker.SetInstIdsAboutToMove(instanceIds);
        }

        public void SetGameObjects(Dictionary<OwnedZone, List<int>> instanceIdsInZones, IReadOnlyCollection<GameCardInZone> newGameObjects)
        {
            if (instanceIdsInZones == null && newGameObjects == null)
                return;

            Log.Debug("instanceIdsInZones: {instanceIdsInZones}", instanceIdsInZones);
            Log.Debug("newGameObjects: {newGameObjects}", newGameObjects);

            if (newGameObjects != null)
            {
                oppCardTracker.CheckForRevealedCards(newGameObjects);

                var newCardsInLib = newGameObjects.Where(o => o.Zone == OwnedZone.MyLibrary && o.IsKnown);
                MyLibraryTracker.RevealCards(newCardsInLib);

                if (instanceIdsInZones == null)
                    return;
            }
            else
            {
                newGameObjects = new GameCardInZone[0];
            }

            // give trackers the opportunity to correct if count or order differs from tracked
            foreach (var instanceInZone in instanceIdsInZones)
            {
                var withGrpIds = instanceInZone.Value == null
                    ? new CardIds[0]
                    : AddGrpIds(instanceInZone.Value, newGameObjects);

                cardsInZones[instanceInZone.Key].SetInstanceIds(withGrpIds);
            }
        }

        public void DrawHands(IReadOnlyDictionary<OwnedZone, List<int>> instanceIdsInZones, IReadOnlyCollection<GameCardInZone> newGameObjects)
        {
            if (!instanceIdsInZones.TryGetValue(OwnedZone.MyHand, out var hand)
                || !instanceIdsInZones.TryGetValue(OwnedZone.MyLibrary, out var lib)
                || lib == null)
                return;

            if (hand == null) hand = new List<int>(0);

            var newHand = AddGrpIds(hand, newGameObjects);

            var cardsToHand = MyLibraryTracker
                .DrawOrMulligan(lib, newHand)
                .ToArray();

            foreach (var c in cardsToHand)
                c.MoveToZone(OwnedZone.MyHand, OwnedZone.MyLibrary);

            cardsInZones[OwnedZone.MyHand].Clear();
            cardsInZones[OwnedZone.MyHand].AddCards(cardsToHand);
        }

        static IReadOnlyCollection<ITrackedCard> AddGrpIds(IEnumerable<int> instanceIds, IEnumerable<GameCardInZone> newGameObjects)
        {
            return instanceIds
                .GroupJoin(newGameObjects,
                    i => i,
                    c => c.InstId,
                    (instId, cards) => new CardIds(instId, cards.SingleOrDefault()?.GrpId ?? 0))
                .ToArray();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(new
            {
                MySeatId,
                OpponentSeatId,
                OpponentScreenName,
                Players,
                OpponentCardsSeen,
                TrackedZones,
            }, Formatting.Indented);
        }
    }
}
