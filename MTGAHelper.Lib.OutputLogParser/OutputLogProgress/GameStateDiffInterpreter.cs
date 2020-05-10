using MTGAHelper.Entity;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.OutputLogProgress
{
    internal class GameStateDiffInterpreter
    {
        ICollection<OutputLogError> Errors = new List<OutputLogError>();
        GameProgress currentGameProgress;
        GameStateMessageResult gsm;
        IReadOnlyDictionary<int, Card> dictAllCards;

        public void Init(GameProgress currentGameProgress, IReadOnlyDictionary<int, Card> cardsByGrpId)
        {
            this.currentGameProgress = currentGameProgress;
            this.dictAllCards = cardsByGrpId;
        }

        public ICollection<OutputLogError> UpdateMatchGameStateMessage_Diff(GameStateMessageResult gsm)
        {
            // Happens when game disconnected and outputlog starts with data from a previous log
            if (currentGameProgress == null)
                return new OutputLogError[0];

            Errors = new List<OutputLogError>();
            this.gsm = gsm;

            if (currentGameProgress.CurrentTurn == 0)
            {
                // Still mulliganing and the library is reinitialized each time...
                currentGameProgress.InitPlayerLibrary(gsm.Raw.gameStateMessage.zones);
            }

            // Set the turn number
            var turnInfo = gsm.Raw.gameStateMessage?.turnInfo;
            if (turnInfo?.turnNumber != null)
                currentGameProgress.CurrentTurn = turnInfo.turnNumber;

            //if (gsm.Timestamp == 636922646535459255)
            //    System.Diagnostics.Debugger.Break();

            //if (gsm.Timestamp == 636896160671918127)
            //    System.Diagnostics.Debugger.Break();

            // Protection
            if (gsm.Raw?.gameStateMessage?.annotations != null)
            {
                RecordIdsChanged();
                AnalyzeCardsMovements();
            }

            DiscoverGameObjects();

            // This requires the CardTransfersByTurn to be set for this message
            DetectMulligans();

            // This requires the RecordIdsChanged
            DetectOpponentCards();

            return Errors;
        }

        void DiscoverGameObjects()
        {
            if (gsm.Raw.gameStateMessage.gameObjects == null)
                return;

            foreach (var o in gsm.Raw.gameStateMessage.gameObjects)
            {
                var library = o.ownerSeatId == currentGameProgress.systemSeatId ? currentGameProgress.Library : currentGameProgress.LibraryOpponent;

                //try
                //{
                var card = library.SingleOrDefault(i => i.Ids.Contains(o.instanceId));
                if (card != null)
                    card.GrpId = o.grpId;
                //}
                //catch (Exception ex)
                //{
                //    System.Diagnostics.Debugger.Break();
                //}
            }
        }

        void DetectOpponentCards()
        {
            // Protection
            if (gsm.Raw?.gameStateMessage?.gameObjects == null)
                return;

            var opponentGameObjects = gsm.Raw.gameStateMessage.gameObjects
                .Where(i => i.type == "GameObjectType_Card")
                .Where(i => i.visibility == "Visibility_Public")
                .Where(i => i.ownerSeatId == currentGameProgress.systemSeatIdOpponent)
                .Where(i => i.isFacedown != true)
                .ToArray();

            var opponentCards = opponentGameObjects
                .Select(i => new CardRevealed
                {
                    instanceId = i.instanceId,
                    grpId = i.grpId,
                    initialInstanceId = currentGameProgress.LibraryOpponent.FirstOrDefault(x => x.Ids.Contains(i.instanceId))?.InitialId
                })
                .ToArray();

            foreach (var c in opponentCards)
            {
                //if (c.grpId == 66423)
                //if (c.grpId == 3)
                //    System.Diagnostics.Debugger.Break();

                var instanceId = c.initialInstanceId ?? c.instanceId;
                var opponentCardsSeenContainsInstanceId = currentGameProgress.OpponentCardsSeenByInstanceId.ContainsKey(c.instanceId);
                var opponentCardsSeenContainsInitialInstanceId = c.initialInstanceId != null && currentGameProgress.OpponentCardsSeenByInstanceId.ContainsKey(c.initialInstanceId.Value);

                if (opponentCardsSeenContainsInstanceId == false && opponentCardsSeenContainsInitialInstanceId == false)
                {
                    currentGameProgress.OpponentCardsSeenByInstanceId.Add(instanceId, c.grpId);
                }
            }

            //if (currentGameProgress.OpponentCardsSeenByInstanceId.Any(i => i.Value == 3))
            //    System.Diagnostics.Debugger.Break();
        }

        void DetectMulligans()
        {
            if (currentGameProgress.CardTransfersByTurn.Any() || gsm.Raw.gameStateMessage.zones == null)
                return;

            var handIds = gsm.Raw.gameStateMessage.zones?.FirstOrDefault(i => i.type == "ZoneType_Hand" && i.ownerSeatId == currentGameProgress.systemSeatId)?.objectInstanceIds;
            var handOpponent = gsm.Raw.gameStateMessage.zones?.FirstOrDefault(i => i.type == "ZoneType_Hand" && i.ownerSeatId == currentGameProgress.systemSeatIdOpponent)?.objectInstanceIds;

            if (handIds != null)
            {
                //switch (gsm.Raw.gameStateMessage.gameInfo.mulliganType)
                //{
                //    case "MulliganType_London":
                //        DetectMulliganLondon();
                //        break;
                //    case "MulliganType_Vancouver":
                //        DetectMulliganVancouver();
                //        break;
                //}

                //var dict = gsm.Raw.gameStateMessage.gameObjects.ToDictionary(i => i.instanceId, i => i.grpId);
                try
                {
                    var dict = currentGameProgress.Library
                        .Where(i => i.GrpId != default(int))
                        .ToDictionary(i => i.Ids.Last(), i => i.GrpId);

                    var handGrpIds = handIds.Select(i => dict[i]).ToArray();
                    if (currentGameProgress.StartingHands.Any(i => i.Count == handGrpIds.Length) == false)
                        currentGameProgress.StartingHands.Add(handGrpIds);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "{outputLogError}: Error in DetectMulligans():", "OUTPUTLOG");
                }
            }

            if (handOpponent != null)
                currentGameProgress.MulliganCountOpponent = 7 - handOpponent.Count;
        }

        void RecordIdsChanged()
        {
            // Protection
            if (gsm.Raw.gameStateMessage?.annotations == null)
                return;

            //try
            //{
            var idsChanged = gsm.Raw.gameStateMessage.annotations
            .Where(i => i.type.Any(x => x == "AnnotationType_ObjectIdChanged"))
            .ToDictionary(
                i => i.details.Single(x => x.key == "orig_id").valueInt32.Single(),
                i => i.details.Single(x => x.key == "new_id").valueInt32.Single()
            );

            foreach (var idChanged in idsChanged)
            {
                //if (idChanged.Key == 421 && idChanged.Value == 430)
                //    System.Diagnostics.Debugger.Break();

                currentGameProgress.UpdateId(gsm.Timestamp, idChanged.Key, idChanged.Value);
            }

            // Shuffle
            var shuffles = gsm.Raw.gameStateMessage.annotations
                .Where(i => i.type.Any(x => x == "AnnotationType_Shuffle"))
                .ToArray();

            foreach (var s in shuffles)
            {
                //var seatId = s.affectedIds.First();

                var oldIds = s.details.Single(i => i.key == "OldIds").valueInt32;
                var newIds = s.details.Single(i => i.key == "NewIds").valueInt32;

                for (int idx = 0; idx < oldIds.Count; idx++)
                    currentGameProgress.UpdateId(gsm.Timestamp, oldIds[idx], newIds[idx]);
            }
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debugger.Break();
            //    //throw;
            //}
        }

        void AnalyzeCardsMovements()
        {
            // Protection
            if (gsm.Raw?.gameStateMessage?.annotations == null)
                return;

            var zoneTransfers = gsm.Raw.gameStateMessage.annotations
                .Where(i => i.type.Any(x => x == "AnnotationType_ZoneTransfer"))
                .SelectMany(i => i.affectedIds.Select(id => new ZoneTransfer
                {
                    instanceId = id,
                    affectorId = i.affectorId,
                    annotationId = i.id,
                    zone_src = new Zone(currentGameProgress.Zones[i.details.Single(x => x.key == "zone_src").valueInt32.Single()], currentGameProgress.systemSeatIdOpponent, currentGameProgress.GetPlayerFromId(id)),
                    zone_dest = new Zone(currentGameProgress.Zones[i.details.Single(x => x.key == "zone_dest").valueInt32.Single()], currentGameProgress.systemSeatIdOpponent, currentGameProgress.GetPlayerFromId(id)),
                }))
                .ToArray();

            foreach (var zt in zoneTransfers)
            {
                AnalyzeSingleZoneTransfer(zt);
            }
        }

        void AnalyzeSingleZoneTransfer(ZoneTransfer zt)
        {
            var gameObject = gsm.Raw.gameStateMessage.gameObjects?.FirstOrDefault(i => i.instanceId == zt.instanceId);
            //if (gameObject != null)
            {
                var playerId = currentGameProgress.GetPlayerFromId(zt.instanceId);
                if (playerId == PlayerEnum.Unknown)
                {
                    // Cannot retrace the ids changes to the original card...probably because of summarized messages

                    if (gameObject != null)
                    {
                        if (gameObject.ownerSeatId == gameObject.controllerSeatId)
                            playerId = gameObject.ownerSeatId == gameObject.controllerSeatId ? (
                                gameObject.ownerSeatId == currentGameProgress.systemSeatId ? PlayerEnum.Me : PlayerEnum.Opponent
                                ) : PlayerEnum.Unknown;
                        else
                        {
                            //System.Diagnostics.Debugger.Break();
                            //Log.Error("OUTPUTLOG {outputLogError}. User {userId}, Match id {matchId}",
                            //    "Unknown zone transfer",

                            //    JsonConvert.SerializeObject(gsm));
                            Errors.Add(new OutputLogError(OutputLogErrorType.UnknownZoneTransfer, gsm.Timestamp));
                            return;
                        }
                    }
                    else
                    {
                        if (zt.zone_src.Player == zt.zone_dest.Player)
                            playerId = zt.zone_dest.Player;
                        else
                        {
                            //System.Diagnostics.Debugger.Break();
                            //Log
                            playerId = zt.zone_dest.Player;
                        }
                    }
                }

                var c = new CardForTurn
                {
                    CardGrpId = gameObject?.grpId ?? default(int),
                    Player = playerId,
                    Turn = currentGameProgress.CurrentTurn,
                };

                if (zt.zone_src.Name == "ZoneType_Library" && zt.zone_dest.Name == "ZoneType_Hand")
                    c.Action = CardForTurnEnum.Drew;
                else if (zt.zone_src.Name == "ZoneType_Hand" && (zt.zone_dest.Name == "ZoneType_Battlefield" || zt.zone_dest.Name == "ZoneType_Stack"))
                    c.Action = CardForTurnEnum.Played;
                else if (zt.zone_src.Name == "ZoneType_Hand" && zt.zone_dest.Name == "ZoneType_Graveyard")
                    c.Action = CardForTurnEnum.Discarded;

                else if (zt.zone_src.Name == "ZoneType_Library" && zt.zone_dest.Name == "ZoneType_Battlefield")
                    c.Action = CardForTurnEnum.FromLibraryToBattlefield;
                else if (zt.zone_src.Name == "ZoneType_Library" && zt.zone_dest.Name == "ZoneType_Exile")
                    c.Action = CardForTurnEnum.FromLibraryToExile;
                else if (zt.zone_src.Name == "ZoneType_Library" && zt.zone_dest.Name == "ZoneType_Graveyard")
                    c.Action = CardForTurnEnum.FromLibraryToGraveyard;

                else if (zt.zone_src.Name == "ZoneType_Battlefield" && (zt.zone_dest.Name == "ZoneType_Graveyard" || zt.zone_dest.Name == "ZoneType_Exile"))
                    c.Action = CardForTurnEnum.PermanentRemoved;

                else if (zt.zone_src.Name == "ZoneType_Stack" && (zt.zone_dest.Name == "ZoneType_Battlefield" || zt.zone_dest.Name == "ZoneType_Graveyard"))
                    c.Action = CardForTurnEnum.SpellResolved;

                else if (zt.zone_src.Name == "ZoneType_Graveyard" && zt.zone_dest.Name == "ZoneType_Battlefield")
                    c.Action = CardForTurnEnum.FromGraveyardToBattlefield;
                else if (zt.zone_src.Name == "ZoneType_Graveyard" && zt.zone_dest.Name == "ZoneType_Hand")
                    c.Action = CardForTurnEnum.FromGraveyardToHand;
                else if (zt.zone_src.Name == "ZoneType_Graveyard" && zt.zone_dest.Name == "ZoneType_Stack")
                    c.Action = CardForTurnEnum.FromGraveyardToStack;
                else if (zt.zone_src.Name == "ZoneType_Graveyard" && zt.zone_dest.Name == "ZoneType_Exile")
                    c.Action = CardForTurnEnum.FromGraveyardToExile;

                else if (zt.zone_src.Name == "ZoneType_Exile" && zt.zone_dest.Name == "ZoneType_Stack")
                    c.Action = CardForTurnEnum.FromExileToStack;
                else if (zt.zone_src.Name == "ZoneType_Exile" && zt.zone_dest.Name == "ZoneType_Battlefield")
                    c.Action = CardForTurnEnum.FromExileToBattlefield;
                else if (zt.zone_src.Name == "ZoneType_Exile" && zt.zone_dest.Name == "ZoneType_Hand")
                    c.Action = CardForTurnEnum.FromExileToHand;

                if (c.Player == 0/* || c.Turn == 14*/)
                {
                    //System.Diagnostics.Debugger.Break();
                    Errors.Add(new OutputLogError(OutputLogErrorType.PlayerZero, gsm.Timestamp));
                    return;
                }

                /*if (c.Action == CardForTurnEnum.Unknown)
                {
                    Errors.Add(new OutputLogError(OutputLogErrorType.CardForTurnUnknown, gsm.Timestamp));
                    return;
                }
                else*/
                if (c.Action == CardForTurnEnum.Drew || c.Action == CardForTurnEnum.Played || c.Action == CardForTurnEnum.Discarded)
                {
                    if (dictAllCards.ContainsKey(c.CardGrpId))
                        currentGameProgress.AddCardTransfer(c);
                }
            }
            //else
            //if (gameObject == null)
            //{
            //    System.Diagnostics.Debugger.Break();
            //}
        }
    }
}
