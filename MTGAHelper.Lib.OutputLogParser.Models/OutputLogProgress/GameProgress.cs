using System;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Entity.MtgaOutputLog;
using Serilog;

namespace MTGAHelper.Lib.OutputLogParser.Models.OutputLogProgress
{
    public class GameProgress
    {
        // Mapped to GameDetail
        public DateTime StartDateTime { get; }
        public long SecondsCount => (long)(LastMessage - StartDateTime).TotalSeconds;
        public GameOutcomeEnum Outcome { get; set; }
        public FirstTurnEnum FirstTurn { get; set; }
        public int MulliganCountOpponent { get; set; } = -1;

        //public ConfigModelRawDeck DeckUsed { get; set; }
        public Dictionary<int, int> DeckCards { get; set; }

        //public Dictionary<int, int> OpponentCardsSeen => CardTransfersByTurn.Values
        //    .SelectMany(i => i)
        //    .Where(i => i.Player == PlayerEnum.Opponent)
        //    .Where(i => i.CardGrpId != default(int))
        //    .GroupBy(i => i.CardGrpId)
        //    .ToDictionary(i => i.Key, i => i.Count());
        public Dictionary<int, int> OpponentCardsSeenByInstanceId { get; } = new Dictionary<int, int>();
        public IList<ICollection<int>> StartingHands { get; } = new List<ICollection<int>>();
        public Dictionary<int, IList<CardForTurn>> CardTransfersByTurn { get; } = new Dictionary<int, IList<CardForTurn>>();

        // For public usage
        public Dictionary<int, GRE.MatchToClient.GameStateMessage.Zone> Zones { get; private set; }
        public int SystemSeatId { get; private set; }
        public int SystemSeatIdOpponent { get; private set; }
        public DateTime LastMessage { get; set; }
        public ICollection<CardIdentifier> Library { get; private set; } = new CardIdentifier[0];
        public ICollection<CardIdentifier> LibraryOpponent { get; private set; } = new CardIdentifier[0];
        public int CurrentTurn { get; set; }

        public GameProgress(DateTime dateStart)
        {
            StartDateTime = dateStart;
            LastMessage = dateStart;
        }

        public bool InitPlayerLibrary(ICollection<GRE.MatchToClient.GameStateMessage.Zone> zones)
        {
            var myLibrary = zones?.FirstOrDefault(i => i.type == "ZoneType_Library" && i.ownerSeatId == SystemSeatId);
            var ids = myLibrary?.objectInstanceIds;

            if (myLibrary == null || ids == null)
                return false;

            if (Library.Any())
            {
                var firstId = myLibrary.objectInstanceIds.FirstOrDefault();
                if (firstId != default(int) && Library.Any(i => i.InitialId == firstId))
                    // Library already established
                    return false;
                else
                    // Library was reinitialized because of mulligan
                    ids.AddRange(zones.First(i => i.type == "ZoneType_Hand" && i.ownerSeatId == SystemSeatId).objectInstanceIds);
            }

            Library = ids.Select(i => new CardIdentifier(i)).ToArray();
            return true;
        }

        public void InitLibraries(ICollection<OutputLogParser.Models.GRE.MatchToClient.GameStateMessage.Zone> zones)
        {
            Zones = zones.ToDictionary(i => i.zoneId, i => i);

            //try
            //{
            if (InitPlayerLibrary(zones))
            {
                var idsOpponent = zones.First(i => i.type == "ZoneType_Library" && i.ownerSeatId == SystemSeatIdOpponent).objectInstanceIds;
                LibraryOpponent = idsOpponent.Select(i => new CardIdentifier(i)).ToArray();
            }

            //}
            //catch (Exception ex)
            //{
            //    var t = ex;
            //}
        }

        public void UpdateId(long timestamp, int oldId, int newId)
        {
            //if (oldId == 314)
            //    System.Diagnostics.Debugger.Break();

            //if (Library.Any(i => i.Ids.Contains(newId)))
            //    System.Diagnostics.Debugger.Break();
            var newIdAlreadyExistingForCard = Library.FirstOrDefault(i => i.Ids.Contains(newId));
            if (newIdAlreadyExistingForCard != null)
            {
                newIdAlreadyExistingForCard.Ids.Remove(newId);
                Log.Warning("{outputLogError}: newId {newId} already existing in Library [{ts}]", "OUTPUTLOG", newId, timestamp);
            }

            newIdAlreadyExistingForCard = LibraryOpponent.FirstOrDefault(i => i.Ids.Contains(newId));
            if (newIdAlreadyExistingForCard != null)
            {
                newIdAlreadyExistingForCard.Ids.Remove(newId);
                Log.Warning("{outputLogError}: newId {newId} already existing in LibraryOpponent [{ts}]", "OUTPUTLOG", newId, timestamp);
            }

            foreach (var c in Library)
            {
                if (c.Ids.Contains(oldId))
                    c.Ids.Add(newId);
            }

            foreach (var c in LibraryOpponent)
                if (c.Ids.Contains(oldId))
                    c.Ids.Add(newId);
        }

        //public bool InstanceIdFromLibrary(int instanceId)
        //{
        //    return GetPlayerFromId(instanceId) > 0;
        //}

        public PlayerEnum GetPlayerFromId(int instanceId)
        {
            if (Library.Any(i => i.Ids.Contains(instanceId)))
                return PlayerEnum.Me;

            if (LibraryOpponent.Any(i => i.Ids.Contains(instanceId)))
                return PlayerEnum.Opponent;

            return PlayerEnum.Unknown;
        }

        public void AddCardTransfer(CardForTurn c)
        {
            if (CardTransfersByTurn.ContainsKey(c.Turn) == false)
                CardTransfersByTurn.Add(c.Turn, new List<CardForTurn>());

            CardTransfersByTurn[c.Turn].Add(c);
        }

        public void SetOpponentId(int opponentSystemId)
        {
            SystemSeatIdOpponent = opponentSystemId;
            SystemSeatId = opponentSystemId == 2 ? 1 : 2;
        }
    }
}
