using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Entity.Config.Users;
using MTGAHelper.Entity.MtgaOutputLog;
using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.Exceptions;
using MTGAHelper.Lib.OutputLogParser.EventsSchedule;
using MTGAHelper.Lib.OutputLogParser.Models;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.ClientToMatch;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.ConnectResp;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.MatchGameRoomStateChanged;
using MTGAHelper.Lib.OutputLogParser.Models.OutputLogProgress;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;
using Newtonsoft.Json;
using Serilog; //using MTGAHelper.Lib.EventsSchedule;

namespace MTGAHelper.Lib.OutputLogParser.OutputLogProgress
{
    public class MtgaOutputLogResultsPreparer
    {
        public string UserId { get; set; } = "Unknown";
        public Guid? ProducedErrorId { get; private set; }

        string PlayerName { get; set; }
        string MyScreenName { get; set; }
        string OpponentNameWithTagNumber { get; set; }

        Dictionary<string, int> DictPreconDecksTileIdById = new Dictionary<string, int>();

        IList<OutputLogError> Errors = new List<OutputLogError>();
        int lastOpponentSystemId;
        //OutputLogStateEnum currentState;
        CourseDeckRaw currentMatchDeckSubmitted;
        MatchResult currentMatch = null;// MatchResult.CreateDefault();
        //GameDetail currentGame;
        GameProgress currentGameProgress { get; set; }

        //readonly ConfigModelApp configApp;
        readonly IMapper mapper;
        readonly IEventTypeCache eventsScheduleManager;
        readonly GameStateDiffInterpreter gameStateDiffInterpreter = new GameStateDiffInterpreter();
        readonly IReadOnlyDictionary<int, Card> dictAllCards;

        List<MatchResult> matches;
        public OutputLogResult Results { get; private set; }

        /// <summary>
        /// To be used with the new SQL data model
        /// </summary>
        public OutputLogResult2 Results2 { get; private set; }
        string currentAccount;


        Dictionary<string, GetEventPlayerCourseV2Result> CourseByEventName = new Dictionary<string, GetEventPlayerCourseV2Result>();

        public MtgaOutputLogResultsPreparer(
            //IOptionsMonitor<ConfigModelApp> configApp,
            CacheSingleton<Dictionary<int, Card>> cacheCards,
            IMapper mapper,
            IEventTypeCache eventsScheduleManager)
        {
            //this.configApp = configApp.CurrentValue;
            dictAllCards = cacheCards.Get();
            this.mapper = mapper;
            this.eventsScheduleManager = eventsScheduleManager;
            Reset();
        }

        internal void Reset()
        {
            ProducedErrorId = default(Guid?);
            MyScreenName = default(string);
            OpponentNameWithTagNumber = default(string);
            Errors = new List<OutputLogError>();
            lastOpponentSystemId = default(int);
            currentMatchDeckSubmitted = default(CourseDeckRaw);
            currentMatch = default(MatchResult);
            currentGameProgress = default(GameProgress);
            matches = new List<MatchResult>();
            Results = new OutputLogResult();
            Results2 = new OutputLogResult2();
        }

        public void AddResult(IMtgaOutputLogPartResult r)
        {
            //if (r.MatchId == "c84b04f4-16da-4abe-9968-4b7866176847") Debugger.Break();

            try
            {
                if (r is IgnoredResult || r is IgnoredMatchResult || r is UnknownResult || r is UnknownMatchResult)
                    return;

                //else if (r is MtgaOutputLogPartResultBase<string> resultNoJson)
                //{
                //    if (resultNoJson.Raw.Contains("MatchCompleted -> Disconnected"))
                //    {
                //        EndCurrentMatch();
                //    }
                //}

                else if (r is ITagMatchResult)
                {
                    //if (configApp.IsFeatureEnabled(ConfigAppFeatureEnum.ParseMatches) == false)
                    //    return;

                    if (r is MatchCreatedResult mc)
                    {
                        CreateMatch(mc);
                    }
                    //else if (r is DuelSceneGameStopResult gameStop)
                    else if (r is GameStateMessageResult gsm && gsm.Raw.gameStateMessage?.gameInfo?.stage == "GameStage_GameOver")
                    {
                        var winningTeamId = gsm.Raw.gameStateMessage.gameInfo.results.First().winningTeamId;
                        UpdateGame(winningTeamId);
                    }
                    //else if (r is DuelSceneSideboardingStopResult)
                    else if (r is ClientToMatchResult<PayloadEnterSideboardingReq>)
                    {
                        EndCurrentGame();
                        //CreateGame()
                    }
                    else if (r is MatchGameRoomStateChangedEventResult roomChanged)
                    {
                        if (roomChanged.Raw.gameRoomInfo.gameRoomConfig.reservedPlayers != null)
                        {
                            var opponentInfo = roomChanged.Raw.gameRoomInfo.gameRoomConfig.reservedPlayers.First(i => i.playerName != MyScreenName);
                            lastOpponentSystemId = opponentInfo.systemSeatId;
                            OpponentNameWithTagNumber = opponentInfo.playerName;
                            CreateGame(roomChanged.LogDateTime, opponentInfo.systemSeatId, null);
                            //currentGameProgress.DeckUsed.Name = currentMatch.DeckUsed.Name;
                        }

                        if (roomChanged.Raw.gameRoomInfo.finalMatchResult != null)
                            EndCurrentMatch(roomChanged.Raw.gameRoomInfo.finalMatchResult);
                    }
                    else if (r is IntermissionReqResult intermission)
                    {
                        if (intermission.Raw.intermissionReq.result.reason == "ResultReason_Concede")
                        {
                            //currentGameProgress.Outcome = (intermission.Raw.intermissionReq.result.winningTeamId == currentGameProgress.systemSeatId) ? GameOutcomeEnum.Victory : GameOutcomeEnum.Defeat;
                            //EndCurrentGame();

                            var c = new CardForTurn
                            {
                                CardGrpId = default(int),
                                Player = intermission.Raw.intermissionReq.result.winningTeamId == currentGameProgress.systemSeatId ? PlayerEnum.Opponent : PlayerEnum.Me,
                                Turn = currentGameProgress.CurrentTurn,
                                Action = CardForTurnEnum.Conceded,
                            };
                            currentGameProgress.AddCardTransfer(c);
                        }
                        else if (intermission.Raw.intermissionReq.result.reason == "ResultReason_Game" &&
                                 intermission.Raw.intermissionReq.result.result == "ResultType_Draw")
                        {
                            currentGameProgress.Outcome = GameOutcomeEnum.Draw;
                        }
                    }
                    else
                        UpdateMatch(r);
                }
                else if (r is AuthenticateResponseResult authenticateResponse)
                {
                    MyScreenName = authenticateResponse.Raw.authenticateResponse.screenName;
                    if (currentMatch != null)
                        currentMatch.StartDateTime = authenticateResponse.LogDateTime;
                }
                else
                    AddResultLobby(r);
            }
            catch (Exception ex)
            {
                ProduceError(ex, false);
            }
        }

        private void ProduceError(Exception ex, bool isManaged)
        {
            if (ProducedErrorId == null)
                ProducedErrorId = Guid.NewGuid();

            if (isManaged)
                Log.Warning(ex, "{outputLogError}: user {userId}, see {fileId}", "OUTPUTLOG Error while parsing outputlog", UserId, ProducedErrorId);
            else
            {
                Log.Error(ex, "{outputLogError}: user {userId}, see {fileId}", "OUTPUTLOG Error while parsing outputlog", UserId, ProducedErrorId);
                if (ex.InnerException != null)
                    Log.Error(ex.InnerException, "{outputLogError}: user {userId}", "OUTPUTLOG Error while parsing outputlog INNER EXCEPTION", UserId);
            }
        }

        private void AddResultLobby(IMtgaOutputLogPartResult result)
        {
            if (result is GetPlayerInventoryResult inventory)
            {
                var info = mapper.Map<Inventory>(inventory.Raw.payload);
                AddToListInfoByDate(Results.InventoryByDate, info, inventory.LogDateTime);
                AppendToListInfoByDate(Results.InventoryIntradayByDate, info, inventory.LogDateTime);

                Results2.ResultsByNameTag[currentAccount].GetPlayerInventoryResults.Add(inventory);
            }
            else if (result is GetSeasonAndRankDetailResult season)
            {
                Results.SeasonAndRankDetail = season.Raw.payload;

                Results2.ResultsByNameTag[currentAccount].GetSeasonAndRankDetailResults.Add(season);
            }
            else if (result is PlayerNameResult playerName)
            {
                Results.PlayerName = playerName.Name;

                currentAccount = playerName.Name;

                if (Results2.ResultsByNameTag.ContainsKey(currentAccount) == false)
                    Results2.ResultsByNameTag[currentAccount] = new OutputLogResult2ByNameTag();
                Results2.ResultsByNameTag[currentAccount].PlayerNameResults.Add(playerName);
            }
            else if (result is RankUpdatedResult rankUpdated)
            {
                AppendToListInfoByDate(Results.RankUpdatedByDate, rankUpdated.Raw.payload, rankUpdated.LogDateTime);
                UpdateRank_Synthetic(rankUpdated);

                Results2.ResultsByNameTag[currentAccount].RankUpdatedResults.Add(rankUpdated);
            }
            else if (result is MythicRatingUpdatedResult mythicRatingUpdated)
            {
                AppendToListInfoByDate(Results.MythicRatingUpdatedByDate, mythicRatingUpdated.Raw.payload, mythicRatingUpdated.LogDateTime);

                Results2.ResultsByNameTag[currentAccount].MythicRatingUpdatedResults.Add(mythicRatingUpdated);
            }
            else if (result is GetPlayerCardsResult collection)
            {
                var info = collection.Raw;
                AddToListInfoByDate(Results.CollectionByDate, info.payload, collection.LogDateTime);

                var mustAppend = true;
                var infoForDate = Results.CollectionIntradayByDate.FirstOrDefault(i => i.DateTime.Date == collection.LogDateTime.Date)?.Info;
                if (infoForDate != null && infoForDate.Any())
                    mustAppend = infoForDate.Last().Value.Sum(x => x.Value) != collection.Raw.payload.Sum(x => x.Value);

                if (mustAppend)
                    AppendToListInfoByDate(Results.CollectionIntradayByDate, info.payload, collection.LogDateTime);

                Results2.ResultsByNameTag[currentAccount].GetPlayerCardsResults.Add(collection);
            }
            else if (result is GetPlayerProgressResult progress)
            {
                var tracks = progress.Raw.payload.expiredBattlePasses
                    .Union(new[] { progress.Raw.payload.activeBattlePass })
                    .Union(new[] { progress.Raw.payload.eppTrack });

                var playerProgresses = mapper.Map<ICollection<PlayerProgress>>(tracks);
                var info = playerProgresses.Where(pp => pp != null).ToDictionary(i => i.TrackName, i => i);
                AddToListInfoByDate(Results.PlayerProgressByDate, info, progress.LogDateTime);

                AppendToListInfoByDate(Results.PlayerProgressIntradayByDate, progress.Raw.payload, progress.LogDateTime);

                Results2.ResultsByNameTag[currentAccount].GetPlayerProgressResults.Add(progress);
            }
            else if (result is GetDecksListResult decks)
            {
                AddToListInfoByDate(Results.MtgaDecksFoundByDate, new HashSet<string>(decks.Raw.payload.Select(i => i.id)), decks.LogDateTime);
                Results.DecksSynthetic = mapper.Map<List<ConfigModelRawDeck>>(decks.Raw.payload);

                Results2.ResultsByNameTag[currentAccount].GetDecksListResults.Add(decks);
            }
            else if (result is GetCombinedRankInfoResult ranks)
            {
                var info = ranks.Raw.payload.ToConfig();
                AddToListInfoByDate(Results.RankSyntheticByDate, info, ranks.LogDateTime);

                AppendToListInfoByDate(Results.CombinedRankInfoByDate, ranks.Raw.payload, result.LogDateTime);

                Results2.ResultsByNameTag[currentAccount].GetCombinedRankInfoResults.Add(ranks);
            }
            else if (result is GetPlayerQuestsResult quests)
            {
                var info = mapper.Map<List<PlayerQuest>>(quests.Raw.payload);
                AddToListInfoByDate(Results.PlayerQuestsByDate, info, quests.LogDateTime);

                Results2.ResultsByNameTag[currentAccount].GetPlayerQuestsResults.Add(quests);
            }
            else if (result is CrackBoostersResult booster)
            {
                AppendToListInfoByDate(Results.CrackedBoostersByDate, booster.Raw.payload, booster.LogDateTime);

                Results2.ResultsByNameTag[currentAccount].CrackBoostersResults.Add(booster);
            }
            else if (result is CompleteVaultResult vault)
            {
                AppendToListInfoByDate(Results.VaultsOpenedByDate, vault.Raw.payload, vault.LogDateTime);

                Results2.ResultsByNameTag[currentAccount].CompleteVaultResults.Add(vault);
            }
            else if (result is PayEntryResult payEntry)
            {
                AppendToListInfoByDate(Results.PayEntryByDate, payEntry.Raw.payload, payEntry.LogDateTime);

                Results2.ResultsByNameTag[currentAccount].PayEntryResults.Add(payEntry);
            }
            else if (result is InventoryUpdatedResult inventoryUpdate)
            {
                AppendToListInfoByDate(Results.InventoryUpdatesByDate, inventoryUpdate.Raw.payload, inventoryUpdate.LogDateTime);

                Results2.ResultsByNameTag[currentAccount].InventoryUpdatedResults.Add(inventoryUpdate);
            }
            else if (result is PostMatchUpdateResult postMatchUpdate)
            {
                AppendToListInfoByDate(Results.PostMatchUpdatesByDate, postMatchUpdate.Raw.payload, postMatchUpdate.LogDateTime);

                Results2.ResultsByNameTag[currentAccount].PostMatchUpdateResults.Add(postMatchUpdate);
            }
            else if (result is EventClaimPrizeResult eventClaimPrize)
            {
                AppendToListInfoByDate(Results.EventClaimPriceByDate, eventClaimPrize.Raw.payload, eventClaimPrize.LogDateTime);

                Results2.ResultsByNameTag[currentAccount].EventClaimPrizeResults.Add(eventClaimPrize);
            }
            else if (result is GetActiveEventsV2Result events)
            {
                eventsScheduleManager.AddEvents(events.Raw.payload);

                Results2.ResultsByNameTag[currentAccount].GetActiveEventsV2Results.Add(events);
            }
            else if (result is GetPreconDecksV3Result preconDecks)
            {
                DictPreconDecksTileIdById = preconDecks.Raw.payload.ToDictionary(i => i.id, i => i.deckTileId ?? 0);

                Results2.ResultsByNameTag[currentAccount].GetPreconDecksV3Results.Add(preconDecks);
            }
            //else if (result is ClientConnectedResult clientConnected)
            //{
            //    MyScreenName = clientConnected.Raw.@params.payloadObject.screenName;
            //}
            else if (result is DraftResultBase draftPick)
            {
                //var info = mapper.Map<IList<DraftPickProgress>>(r);
                //AddToListInfoByDate(Results.DraftPickProgressByDate, info, logDateTime);

                //var infoForDate = Results.DraftPickProgressByDate.FirstOrDefault(i => i.DateTime.Date == result.LogDateTime.Date);
                //if (infoForDate == null)
                //{
                //    infoForDate = new InfoByDate<List<DraftMakePickRaw>>(result.LogDateTime, new List<DraftMakePickRaw>());
                //    Results.DraftPickProgressByDate.Add(infoForDate);
                //}

                ////var info = mapper.Map<DraftPickProgress>(r);
                //infoForDate.Info.Add(draftPick.Raw.payload);



                AppendToListInfoByDate(Results.DraftPickProgressIntradayByDate, draftPick.Raw.payload, result.LogDateTime);

                Results2.ResultsByNameTag[currentAccount].ResultDraftPicks.Add(draftPick);
            }
            else if (result is DeckSubmitResult ds)
            {
                EndCurrentMatch();
                currentMatchDeckSubmitted = ds.Raw.payload.CourseDeck;
            }
            else if (result is GetEventPlayerCourseV2Result getCourse)
            {
                //CourseByEventName[getCourse.Raw.InternalEventName] = getCourse;
                currentMatchDeckSubmitted = getCourse.Raw.payload.CourseDeck;

                Results2.ResultsByNameTag[currentAccount].GetEventPlayerCourseV2Results.Add(getCourse);
            }
            //else if (result is LogInfoRequestResult logInfo)
            //{
            //    if (logInfo.Raw.request.Contains("Client.Connected"))
            //    {
            //        var clientConnectedInfo = JsonConvert.DeserializeObject<ClientConnectedRaw>(logInfo.Raw.request);
            //        MyScreenName = clientConnectedInfo.@params.payloadObject.screenName;

            //    }
            //}
        }

        void UpdateRank_Synthetic(RankUpdatedResult rankUpdated)
        {
            var ru = rankUpdated.Raw.payload;
            if (Enum.TryParse(ru.rankUpdateType, out RankFormatEnum format))
            {
                var currentRankInfo = Results.RankSyntheticByDate.FirstOrDefault(i => i.DateTime.Date == rankUpdated.LogDateTime.Date);
                var currentRank = currentRankInfo?.Info?.FirstOrDefault(i => i.Format == format);

                if (currentRank == null)
                {
                    currentRank = new ConfigModelRankInfo(format)
                    {
                        Class = ru.newClass,
                        Level = ru.newLevel,
                        Step = ru.newStep,
                    };

                    if (currentRankInfo == null)
                        currentRankInfo = new InfoByDate<List<ConfigModelRankInfo>>(rankUpdated.LogDateTime, new List<ConfigModelRankInfo> { currentRank });
                    else
                        currentRankInfo.Info.Add(currentRank);
                }
                else
                {
                    currentRankInfo.DateTime = rankUpdated.LogDateTime;
                    currentRank.Class = ru.newClass;
                    currentRank.Level = ru.newLevel;
                    currentRank.Step = ru.newStep;
                }
            }
        }

        private void CreateMatch(MatchCreatedResult r)
        {
            EndCurrentMatch();

            currentMatch = mapper.Map<MatchResult>(r);

            if (currentMatch.DeckUsed == null && currentMatchDeckSubmitted != null)
                currentMatch.DeckUsed = mapper.Map<ConfigModelRawDeck>(currentMatchDeckSubmitted);
        }

        private void CreateGame(DateTime logDateTime, int? opponentSystemId, ConfigModelRawDeck deckUsed)
        {
            if (opponentSystemId.HasValue)
            {
                if (currentGameProgress == null)
                {
                    // MatchGameRoomStateChangedEvent received first
                    currentGameProgress = new GameProgress(opponentSystemId == 2, logDateTime);
                }
                else
                {
                    currentGameProgress.systemSeatIdOpponent = opponentSystemId.Value;
                    currentGameProgress.systemSeatId = opponentSystemId == 2 ? 1 : 2;
                }
            }
            else if (deckUsed != null)
            {
                if (currentGameProgress == null)
                {
                    // GREMessageType_ConnectResp received first
                    currentGameProgress = new GameProgress(deckUsed, logDateTime);
                }
                else
                {
                    currentGameProgress.DeckCards = deckUsed.CardsMain;
                }
            }
        }

        private void UpdateMatch(IMtgaOutputLogPartResult r)
        {
            //try
            //{
            if (/*r.IsMessageSummarized == false &&*/ r is GameStateMessageResult gsm)
                UpdateMatchGameStateMessage(gsm);

            else if (r is ConnectRespResult cr)
            {
                var deckUsed = GetDeckUsed(cr.Raw.connectResp.deckMessage);
                CreateGame(cr.LogDateTime, null, deckUsed);
            }
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debugger.Break();
            //    throw;
            //}
        }

        private ConfigModelRawDeck GetDeckUsed(DeckMessage deckMessage)
        {
            var mainDeck = deckMessage.deckCards
            .GroupBy(i => i)
            .Where(i => i.Count() > 0)
            .ToDictionary(i => i.Key, i => i.Count());

            var sideboard = new Dictionary<int, int>();
            if (deckMessage.sideboardCards != null)
            {
                sideboard = deckMessage.sideboardCards
                   .GroupBy(i => i)
                   .ToDictionary(i => i.Key, i => i.Count());
            }

            return new ConfigModelRawDeck
            {
                //Name = currentMatch?.EventName?.Contains("Draft") == true ? "Draft deck" : currentMatch?.EventName?.Contains("Sealed") == true ? "Sealed deck" : "Other",
                Name = GetDeckNameFromEventType(),
                CardsMain = mainDeck,
                CardsSideboard = sideboard
            };
        }

        private void UpdateMatchGameStateMessage(GameStateMessageResult gsm)
        {
            //try
            //{
            // Happens when game disconnected and outputlog starts with data from a previous log
            if (currentMatch == null)
                return;

            //try
            //{
            if (gsm.Raw.gameStateMessage.type == "GameStateType_Full"
            //&& (
            //gsm.Raw.gameStateMessage.gameStateId == 1 ||    // First game
            //currentGameProgress == null                     // New game in a Bo3

            //)
            )
            {
                //var matchOngoing = matches.FirstOrDefault(i => i.Id == gsm.MatchId);
                //if (matchOngoing != null)
                if (gsm.Raw.msgId == 1 || currentMatch.MatchId == gsm.MatchId)
                {
                    CreateGame(gsm.LogDateTime, lastOpponentSystemId, null);
                }

                currentGameProgress.InitLibraries(gsm.Raw.gameStateMessage.zones);
                gameStateDiffInterpreter.Init(currentGameProgress, dictAllCards);
            }
            else if (gsm.Raw.gameStateMessage.type == "GameStateType_Diff")
            {
                var errors = gameStateDiffInterpreter.UpdateMatchGameStateMessage_Diff(gsm);

                foreach (var err in errors)
                {
                    err.MatchId = currentMatch.MatchId;
                    Errors.Add(err);
                }
            }
            //}
            //catch (OutputLogMessageException ex)
            //{
            //    ProduceError(ex);
            //}

            //try
            //{
            if (currentGameProgress != null)
                currentGameProgress.LastMessage = gsm.LogDateTime;
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debugger.Break();
            //}
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debugger.Break();
            //}

        }

        //private void UpdateGame(DuelSceneGameStopPayloadObjectRaw gameStopInfo)
        private void UpdateGame(int winningTeamId)
        {
            if (currentGameProgress != null)
            {
                try
                {
                    //// Determine who went first
                    //if (gameStopInfo.startingTeamId == currentGameProgress.systemSeatId)
                    //    currentGameProgress.FirstTurn = FirstTurnEnum.Play;
                    //else if (gameStopInfo.startingTeamId == currentGameProgress.systemSeatIdOpponent)
                    //    currentGameProgress.FirstTurn = FirstTurnEnum.Draw;
                    if (currentGameProgress.CardTransfersByTurn.Any())
                    {
                        var firstThingHappened = currentGameProgress.CardTransfersByTurn.First().Value.FirstOrDefault();
                        if (firstThingHappened != null)
                        {
                            currentGameProgress.FirstTurn = firstThingHappened.Player == PlayerEnum.Me ? FirstTurnEnum.Play : FirstTurnEnum.Draw;
                        }
                    }

                    // Determine who won
                    if (winningTeamId != 0)
                    {
                        // winningReason is "ResultReason_Force"
                        // and gameStopInfo.winningTeamId is 0
                        // means the game connected but never started and quit without anyone playing

                        if (winningTeamId == currentGameProgress.systemSeatId)
                            currentGameProgress.Outcome = GameOutcomeEnum.Victory;
                        else if (winningTeamId == currentGameProgress.systemSeatIdOpponent)
                            currentGameProgress.Outcome = GameOutcomeEnum.Defeat;
                    }

                }
                catch (NullReferenceException ex)
                {
                    Log.Error(ex, "{outputLogError} in UpdateGame (winningTeamId:{winningTeamId}):", "OUTPUTLOG error", winningTeamId);
                }
                // Read mulliganed hand info
                //currentGameProgress.MulliganCountOpponent
            }
            //else
            //{
            //    //var game = 
            //    System.Diagnostics.Debugger.Break();
            //}
        }

        private void EndCurrentGame()
        {
            if (currentMatch != null && currentGameProgress != null)
            {
                //var test = currentGameProgress.OpponentCardsSeenByInstanceId.Values.GroupBy(i => i).ToDictionary(i => i.Key, i => i.Count());

                var g = mapper.Map<GameDetail>(currentGameProgress);

                //if (g.OpponentCardsSeen.ContainsKey(3))
                //    System.Diagnostics.Debugger.Break();

                currentMatch.Games.Add(g);
                currentGameProgress = null;
            }
        }

        private void EndCurrentMatch(FinalMatchResultRaw finalResult = null)
        {
            if (finalResult != null)
            {
                var lastGameOutcome = finalResult.resultList.LastOrDefault(i => i.scope == "MatchScope_Game");
                if (lastGameOutcome != null && currentGameProgress != null)
                {
                    if (lastGameOutcome.winningTeamId == currentGameProgress.systemSeatId)
                        currentGameProgress.Outcome = GameOutcomeEnum.Victory;
                    else if (lastGameOutcome.winningTeamId == currentGameProgress.systemSeatIdOpponent)
                        currentGameProgress.Outcome = GameOutcomeEnum.Defeat;
                }
            }

            EndCurrentGame();

            if (currentMatch != null)
            {
                if (OpponentNameWithTagNumber != null)
                    currentMatch.Opponent.ScreenName = OpponentNameWithTagNumber;

                //if (currentMatch?.DeckUsed?.Name == null)
                //    System.Diagnostics.Debugger.Break();
                if (currentMatchDeckSubmitted == null)
                {
                    // When the original submitted deck is not available (i.e. Sealed that was started in a previous log file), take it from the first game of the match
                    //currentMatch.DeckUsed = currentMatch.Games.FirstOrDefault()?.DeckUsed;
                    currentMatch.DeckUsed = mapper.Map<ConfigModelRawDeck>(currentMatchDeckSubmitted);
                }
                else
                {
                    currentMatch.DeckUsed = mapper.Map<ConfigModelRawDeck>(currentMatchDeckSubmitted);
                    currentMatch.DeckUsed.Name = GetDeckNameFromEventType(currentMatch.DeckUsed.Name);
                    currentMatchDeckSubmitted = null;
                }

                matches.Add(currentMatch);
                currentMatch = null;// MatchResult.CreateDefault();
            }
        }

        string GetDeckNameFromEventType(string name = null)
        {
            switch (currentMatch?.EventType)
            {
                case "Limited":
                    return "Limited deck";
                case "Sealed":
                    return "Sealed deck";
                default:
                    return name ?? "N/A";
            }
        }

        public void FinalizeResults(uint lastUploadHash, ICollection<ConverterUsage> converterUsage)
        {
            Results2.LogReadersUsage = converterUsage
                .OrderByDescending(i => i.LastUsed)
                .GroupBy(i => new { i.Converter, i.LogTextKey, i.Prefix, i.Result })
                .Select(i => i.First())
                .ToArray();

            Results.LastUploadHash = lastUploadHash;
            EndCurrentMatch();
            SetMatches(matches);

            if (Errors.Any())
            {
                var errorsMsg = Errors.Select(i => JsonConvert.SerializeObject(i)).ToArray();
                var ex = new OutputLogMessageException(string.Join(Environment.NewLine, errorsMsg));
                ProduceError(ex, true);
            }
        }

        void AppendToListInfoByDate<T>(IList<InfoByDate<Dictionary<DateTime, T>>> listInfoByDate, T newInfo, DateTime newInfoDate)
        {
            var infoForDate = listInfoByDate.FirstOrDefault(i => i.DateTime.Date == newInfoDate.Date);
            if (infoForDate == null)
            {
                listInfoByDate.Add(
                    new InfoByDate<Dictionary<DateTime, T>>(newInfoDate.Date, new Dictionary<DateTime, T>
                    {
                        { newInfoDate, newInfo }
                    }));
            }
            else if (infoForDate.Info.ContainsKey(newInfoDate) == false)
            {
                infoForDate.Info.Add(newInfoDate, newInfo);
            }
            else
            {
                var newInfoStr = newInfoDate.ToString("yyyyMMddHHmmss") + JsonConvert.SerializeObject(newInfo);
                var newInfoHash = Fnv1aHasher.To32BitFnv1aHash(newInfoStr);

                if (infoForDate.Info
                    .Select(i => Fnv1aHasher.To32BitFnv1aHash(i.Key.ToString("yyyyMMddHHmmss") + JsonConvert.SerializeObject(i.Value)))
                    .Contains(newInfoHash) == false)
                {
                    var newDate = newInfoDate.AddSeconds(1);
                    while (infoForDate.Info.ContainsKey(newDate))
                        newDate = newDate.AddSeconds(1);

                    infoForDate.Info.Add(newDate, newInfo);
                }
            }
        }

        void AddToListInfoByDate<T>(IList<InfoByDate<T>> listInfoByDate, T newInfo, DateTime newInfoDate)
        {
            var infoForDate = listInfoByDate.FirstOrDefault(i => i.DateTime.Date == newInfoDate.Date);

            if (infoForDate == null)
                listInfoByDate.Add(new InfoByDate<T>(newInfoDate, newInfo));
            else if (newInfoDate > infoForDate.DateTime)
            {
                infoForDate.DateTime = newInfoDate;
                infoForDate.Info = newInfo;
            }
        }

        void SetMatches(IList<MatchResult> matches)
        {
            Results.MatchesByDate =
                matches.GroupBy(i => i.StartDateTime.Date)
                    .Select(i => new InfoByDate<List<MatchResult>>(i.Last().StartDateTime, i.ToList()))
                    .ToArray()
            ;
        }
    }
}
