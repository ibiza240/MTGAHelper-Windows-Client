using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.Exceptions;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.ClientToMatch;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.ClientToMatch;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class ReaderMtgaOutputLogUnityCrossThreadLogger : ReaderMtgaOutputLogJsonParser, IReaderMtgaOutputLogPart
    {
        //public const string UnityCrossThreadLogger_DuelSceneSideboardingStart = "DuelScene.SideboardingStart";
        //public const string UnityCrossThreadLogger_DuelSceneSideboardingStop = "DuelScene.SideboardingStop";
        //public const string UnityCrossThreadLogger_DuelSceneGameStop = "DuelScene.GameStop";
        //public const string UnityCrossThreadLogger_ClientConnected = "Client.Connected";
        const string PREFIX_MATCH_TO_CLIENT = "Match to ";
        const string PREFIX_CLIENT_TO_MATCH = " to Match";

        readonly Dictionary<string, IReaderMtgaOutputLogJsonBase> converters = new Dictionary<string, IReaderMtgaOutputLogJsonBase>();

        readonly string[] skipped = new[]
        {
            "<== Log",
            "Card #",
        };

        readonly string[] skippedGRE = new[]
        {
            "GREMessageType_UIMessage",
            "GREMessageType_PromptReq",
            "GREMessageType_GetSettingsResp",
            "GREMessageType_TimerStateMessage",
            "GREMessageType_ActionsAvailableReq",
            "GREMessageType_OptionalActionMessage",
        };

        readonly string[] ignoredMatch = new[]
        {
            "GREConnection.HandleWebSocketClosed",
            "NULL entity on ",
        };

        readonly string[] ignored = new[]
        {
            "Got non-message event",
            "FrontDoor",
            "GetCatalogStatus",
            //"Incoming Log.Info True",
            "Log.Error",
            "Error response for message",
            "Mercantile.",
            "GetDeckLimit",
            "GetProductCatalog",
            "GetPlayerArtSkins",
            //"GetPreconDecks",
            //"GetPlayerProgress",
            //"GetAllTracks",
            "Beginning to queue outgoing messages",
            "queued messages",
            "GetMotD",
            "JoinEventQueueStatus",
            "GetTrackDetail",
            "GetActiveMatches",
            "GetPlayerSequenceData",
            //"Event.Join",
            "Event.Draft",
            //"STATE CHANGED",
            "Skins seen",
            "Received unhandled GREMessageType",
            "NULL entity on",
            "Quest.Updated",
            "TrackProgress.Updated",
            "Code.Redeem",
            "RedeemWildCardBulk",
            "UpdateDeckV3",
            "but that card did not exist in the GameState",
            "Ready to sideboard",
            "unhandled Surveil result",
            "CreateDeckV3",
            "DeleteDeck(",
            "Quest.SwapPlayerQuest",
            //"PlayerInventory.CrackBoostersV3",
            //"PlayerInventory.CompleteVault",
            "WebSocketClient",
            "Event.Drop",
            "DirectGame.Challenge",
            "DirectGame.Cancel",
            "Event.ClaimNoGateRewards",
            "Event.AIPractice",
            "Progression.SpendOrb",
            "ClientToMatchServiceMessageType_AuthenticateRequest",
            "PlayerInventory.UpdateBasicLandSet",
            "PlayerInventory.SetPetSelection",
            //"GREConnection.HandleWebSocketClosed",
            "ClientToMatchServiceMessageType_ClientToMatchDoorConnectRequest",
            //"ConnectResp",
            "System.Message",
            "Store.GetEntitlements",
            "Event.SubmitDeckFromChoices",
            "Survey.",
            "Carousel.DisabledItems",
            "Renewal.GetCurrentRenewalDefinition",
            "Event.LeaveQueue",
            //"Event.GetPlayerCourses",
            //"Event.GetPlayerCoursesV2",
            " MD ping",
            " FD ping",
            "Connecting to matchId",
            "SetAvatarSelection",
            "Social.",

            // TO PARSE EVENTUALLY
            //"PayEntry",
            "PlayerInventory.PurchaseCosmetic",
            "PlayerInventory.GetFormats",
            "GetRewardSchedule",

            // GOOD TO PARSE TO GET INFO, SAME FOR EVERYONE
            //"GetSeasonAndRankDetail",
            "GetEventAndSeasonPayouts",
        };

        public ReaderMtgaOutputLogUnityCrossThreadLogger(
            GetDecksListV3Converter converterGetDecksListV3,
            GetCombinedRankInfoConverter converterGetCombinedRankInfo,
            GetPlayerV3CardsConverter converterGetPlayerCardsV3,
            GetPlayerInventoryConverter converterGetPlayerInventory,
            MatchCreatedConverter converterMatchCreated,
            RankUpdatedConverter converterRankUpdated,
            InventoryUpdatedConverter converterInventoryUpdated,
            GetActiveEventsV2Converter converterGetActiveEventsV2,
            //DuelSceneGameStopConverter converterDuelSceneGameStop,
            //DuelSceneSideboardingStartConverter duelSceneSideboardingStartConverter,
            //DuelSceneSideboardingStopConverter duelSceneSideboardingStopConverter,
            DeckSubmitConverter converterDeckSubmit,
            //ClientConnectedConverter clientConnectedConverter,
            ReaderMtgaOutputLogGreMatchToClient converterGreMatchToClient,
            // Migrated from ReaderMtgaOutputLogGreMatchToClient
            IntermissionReqConverter converterIntermissionReq,
            ConnectRespConverter converterConnectResp,
            MulliganReqConverter converterMulliganReq,
            GameStateMessageConverter converterGameStateMessage,
            SubmitDeckReqConverter converterSubmitDeckAfterGame1,
            MythicRatingUpdatedConverter mythicRatingUpdatedConverter,
            AuthenticateResponseConverter authenticateResponseConverter,
            GetEventPlayerCourseV2Converter getEventPlayerCourseV2Converter,
            GetEventPlayerCoursesV2Converter getEventPlayerCoursesV2Converter,
            LogInfoRequestConverter logInfoRequestConverter,
            CompleteDraftConverter completeDraftConverter,
            DraftStatusConverter draftStatusConverter,
            DraftMakePickConverter draftMakePickConverter,
            StateChangedConverter stateChangedConverter,
            GetPreconDecksV3Converter getPreconDecksConverter,
            ClientToMatchConverterGeneric clientToMatchConverterGeneric,
            GetSeasonAndRankDetailConverter getSeasonAndRankDetailConverter,
            GetPlayerProgressConverter getPlayerProgressConverter,
            PayEntryConverter payEntryConverter,
            ProgressionGetAllTracksConverter progressionGetAllTracksConverter,
            EventClaimPrizeConverter eventClaimPrizeConverter,
            GetPlayerQuestsConverter getPlayerQuestsConverter,
            PostMatchUpdateConverter postMatchUpdateConverter,
            CrackBoostersConverter crackBoostersConverter,
            CompleteVaultConverter completeVaultConverter,
            JoinPodmakingConverter JoinPodmakingConverter,
            MakeHumanDraftPickConverter makeHumanDraftPickConverter
        )
        {
            converters.Add("GetActiveEventsV2", converterGetActiveEventsV2);
            converters.Add("GetDeckListsV3", converterGetDecksListV3);
            converters.Add("GetCombinedRankInfo", converterGetCombinedRankInfo);
            converters.Add("GetPlayerCardsV3", converterGetPlayerCardsV3);
            converters.Add("GetPlayerInventory", converterGetPlayerInventory);
            converters.Add("<== Event.MatchCreated", converterMatchCreated);
            converters.Add("==> Event.JoinPodmaking", JoinPodmakingConverter);
            converters.Add("Event.DeckSubmit", converterDeckSubmit);
            converters.Add("==> Rank.Updated", converterRankUpdated);
            converters.Add("<== Inventory.Updated", converterInventoryUpdated);
            //converters.Add(UnityCrossThreadLogger_DuelSceneGameStop, converterDuelSceneGameStop);
            //converters.Add(UnityCrossThreadLogger_DuelSceneSideboardingStart, duelSceneSideboardingStartConverter);
            //converters.Add(UnityCrossThreadLogger_DuelSceneSideboardingStop, duelSceneSideboardingStopConverter);
            //converters.Add(UnityCrossThreadLogger_ClientConnected, clientConnectedConverter);
            converters.Add("AuthenticateResponse", authenticateResponseConverter);  // Order is important, `AuthenticateResponse` has priority over `PREFIX_MATCH_TO_CLIENT`
            converters.Add(PREFIX_MATCH_TO_CLIENT, converterGreMatchToClient);
            converters.Add(PREFIX_CLIENT_TO_MATCH, clientToMatchConverterGeneric);
            converters.Add("MythicRating.Updated", mythicRatingUpdatedConverter);
            converters.Add("Event.GetPlayerCourseV2", getEventPlayerCourseV2Converter);
            converters.Add("Event.GetPlayerCoursesV2", getEventPlayerCoursesV2Converter);
            converters.Add("<== Event.CompleteDraft", completeDraftConverter);
            converters.Add("<== Draft.DraftStatus", draftStatusConverter);
            converters.Add("<== Draft.MakePick", draftMakePickConverter);
            converters.Add("STATE CHANGED", stateChangedConverter);
            converters.Add("Deck.GetPreconDecks", getPreconDecksConverter);
            converters.Add("GetPlayerProgress", getPlayerProgressConverter);
            converters.Add("Event.GetSeasonAndRankDetail", getSeasonAndRankDetailConverter);
            converters.Add("Event.PayEntry", payEntryConverter);
            converters.Add("ClaimPrize", eventClaimPrizeConverter);
            converters.Add("Progression.GetAllTracks", progressionGetAllTracksConverter);
            converters.Add("GetPlayerQuests", getPlayerQuestsConverter);
            converters.Add("<== PostMatch.Update", postMatchUpdateConverter);
            converters.Add("PlayerInventory.CrackBoostersV3", crackBoostersConverter);
            converters.Add("PlayerInventory.CompleteVault", completeVaultConverter);
            converters.Add("==> Draft.MakeHumanDraftPick", makeHumanDraftPickConverter);
            converters.Add("==> Log", logInfoRequestConverter);                     // Order is important, some events now are in requests (MatchCreated,....)

            // After the Client GRE messages disappeared in July 25 patch
            converters.Add(ReaderMtgaOutputLogGreMatchToClient.GREMessageType_IntermissionReq, converterIntermissionReq);
            //converters.Add(ReaderMtgaOutputLogGreMatchToClient.GREMessageType_ConnectResp, converterConnectResp);
            converters.Add(ReaderMtgaOutputLogGreMatchToClient.GREMessageType_ConnectResp, converterConnectResp);
            converters.Add(ReaderMtgaOutputLogGreMatchToClient.GREMessageType_MulliganReq, converterMulliganReq);
            converters.Add(ReaderMtgaOutputLogGreMatchToClient.GREMessageType_GameStateMessage, converterGameStateMessage);
            converters.Add(ReaderMtgaOutputLogGreMatchToClient.GREMessageType_QueuedGameStateMessage, converterGameStateMessage);
            converters.Add(ReaderMtgaOutputLogGreMatchToClient.GREMessageType_SubmitDeckReq, converterSubmitDeckAfterGame1);
        }

        public ICollection<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            //if (part.Contains("==> Event.JoinPodmaking"))
            //    System.Diagnostics.Debugger.Break();

            //var test = ignored.Where(i => part.Contains(i)).ToArray();

            if (skipped.Any(i => part.Contains(i)))
                return null;

            if (ignoredMatch.Any(i => part.Contains(i)))
                return new[] { new IgnoredMatchResult() };

            var isIgnoredResult =
                //part.Contains(UnityCrossThreadLogger_DuelSceneGameStop) == false &&
                //part.Contains(UnityCrossThreadLogger_DuelSceneSideboardingStart) == false &&
                //part.Contains(UnityCrossThreadLogger_DuelSceneSideboardingStop) == false &&
                //part.Contains(UnityCrossThreadLogger_ClientConnected) == false &&
                ((part.Contains("==>") &&
                    part.Contains("==> Log") == false &&
                    part.Contains("==> Draft.MakeHumanDraftPick") == false &&
                    part.Contains("==> Event.JoinPodmaking") == false &&
                    part.Contains("==> Rank.Updated") == false
                ) || ignored.Any(i => part.Contains(i)));

            if (isIgnoredResult)
            {
                return part.Contains("==>")
                    ? new[] { new IgnoredResultRequestToServer() }
                    : new[] { new IgnoredResult() };
            }

            var (converterKey, startIndex) = GetConverter(part);
            var reader = converters[converterKey];

            //if (part.Contains("Y2Q5IJ3TOVB63K5GQS6DJVJXZA to Match")) System.Diagnostics.Debugger.Break();

            if (reader.IsJson)
            {
                try
                {
                    var json = GetJson(part, startIndex);
                    ICollection<IMtgaOutputLogPartResult> ret = new IMtgaOutputLogPartResult[0];

                    if (reader is IReaderMtgaOutputLogJsonMulti readerMulti)
                        ret = readerMulti.ParseJsonMulti(json);
                    else if (reader is IReaderMtgaOutputLogJson readerSingle)
                        ret = new[] { readerSingle.ParseJson(json) };

                    ret = ret
                        .Where(i => i.SubPart == null || skippedGRE.Any(x => i.SubPart.Contains(x)) == false)
                        .ToArray();

                    return ret;

                    throw new InvalidOperationException("Converter not recognized");
                }
                catch (MtgaOutputLogInvalidJsonException ex)
                {
                    if (part.Contains("GetPlayerQuests") == false)
                    {
                        Log.Warning("JsonReader {jsonReader} could not find json in part: <<{part}>>", reader.GetType().ToString(), part);
                    }

                    return new[] { new IgnoredResult() };
                }
                catch (Exception ex)
                {
                    var functioName = $"{reader.GetType()}.ParseJson";
                    //Log.Error(ex, "{outputLogError}: Error on {functioName} with json {json}", "OUTPUTLOG", functioName, json);
                    Log.Error(ex, "{outputLogError}: Error on {functioName} with json", "OUTPUTLOG", functioName);
                    return new IMtgaOutputLogPartResult[0];
                }
            }
            else
            {
                if (reader is StateChangedConverter)
                    return new[] { new StateChangedResult { Raw = part } };
                //else if (reader is ReaderIgnoredClientToMatch)
                //    return new[] { new IgnoredClientToMatchResult { Raw = part } };

                // Default
                return new[] {
                    new MtgaOutputLogPartResultBase<string> { Raw = part }
                };
            }
        }

        (string converterKey, int startIndex) GetConverter(string part)
        {
            foreach (var converter in converters)
            {
                var startIndex = GetPartTypeIndex(part, converter.Key);
                if (startIndex >= 0)
                    return (converter.Key, startIndex);
            }

            throw new MtgaOutputLogUnknownMessageException(part);
        }
    }
}
