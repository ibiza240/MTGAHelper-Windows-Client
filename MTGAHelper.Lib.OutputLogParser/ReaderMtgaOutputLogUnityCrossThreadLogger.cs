using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MTGAHelper.Lib.OutputLogParser.Exceptions;
using MTGAHelper.Lib.OutputLogParser.Models;
using MTGAHelper.Lib.OutputLogParser.Readers;
using Serilog;

namespace MTGAHelper.Lib.OutputLogParser
{
    public class ReaderMtgaOutputLogUnityCrossThreadLogger : IReaderMtgaOutputLogPart
    {
        public string LogTextKey => Constants.LOGTEXTKEY_UNKNOWN;

        readonly IReadOnlyCollection<ILogMessageReader> readers;

        readonly string[] skipped = new[]
        {
            "<== Log",
            "Card #",
        };

        readonly string[] ignoredMatch = new[]
        {
            "GREConnection.HandleWebSocketClosed",
            "NULL entity on ",
        };

        readonly string[] ignored = new[]
        {
            /*^*/"Got non-message event",
            /*^*/"FrontDoor",
            /*^*/"Client",
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
            "Event.GetActiveMatches",// in case we ever want to read: appears before first time in log, so would need AddMessageEvenIfDateNullAttribute
            "GetPlayerSequenceData",
            //"Event.Join",
            "Event.Draft",
            //"STATE CHANGED",
            "Skins seen",
            /*^*/"Received unhandled GREMessageType",
            "NULL entity on",
            "Quest.Updated",
            "TrackProgress.Updated",
            "Code.Redeem",
            "RedeemWildCardBulk",
            "UpdateDeckV3",// "<== Deck.UpdateDeckV3"
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
            "Event.Join",
            "Event.JoinQueue",
            "Event.LeaveQueue",
            //"Event.GetPlayerCourses",
            //"Event.GetPlayerCoursesV2",
            " MD ping",
            " FD ping",
            "Connecting to matchId",
            "SetAvatarSelection",
            "Social.",
            "<== CampaignGraph.GetPlayerState",

            // TO PARSE EVENTUALLY
            //"PayEntry",
            "PlayerInventory.PurchaseCosmetic",
            "PlayerInventory.GetFormats",// in case we ever want to read: appears before first time in log, so would need AddMessageEvenIfDateNullAttribute
            "GetRewardSchedule",

            // GOOD TO PARSE TO GET INFO, SAME FOR EVERYONE
            //"GetSeasonAndRankDetail",
            "GetEventAndSeasonPayouts",
            "<== PlayerInventory.GetCardMetaDataInfo",
            "<== CampaignGraph.GetDefinitions",
        };

        public ReaderMtgaOutputLogUnityCrossThreadLogger(
            //MythicRatingUpdatedConverter mythicRatingUpdatedConverter,
            //CompleteVaultConverter completeVaultConverter,
            IEnumerable<IMessageReaderUnityCrossThreadLogger> readers
        )
        {
            //converters.Add("MythicRating.Updated", mythicRatingUpdatedConverter);
            //converters.Add("PlayerInventory.CompleteVault", completeVaultConverter);
            this.readers = readers.ToArray();
        }

        public ICollection<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            Debug.Assert(readers.Count(r => r.DoesParse(part)) <= 1);
            var reader = readers.FirstOrDefault(c => c.DoesParse(part));
            if (reader != null)
                try
                {
                    return reader.ParsePart(part).ToArray();
                }
                catch (MtgaOutputLogInvalidJsonException)
                {
                    if (part.Contains("GetPlayerQuests") == false)
                    {
                        Log.Warning("JsonReader {jsonReader} could not find json in part: <<{part}>>",
                            reader.GetType().ToString(), part);
                    }

                    return new[] { new IgnoredResult() { LogTextKey = reader.LogTextKey } };
                }
                catch (Exception ex)
                {
                    var functioName = $"{reader.GetType()}.ParseJson";
                    //Log.Error(ex, "{outputLogError}: Error on {functioName} with json {json}", "OUTPUTLOG", functioName, json);
                    Log.Error(ex, "{outputLogError}: Error on {functioName} with json", "OUTPUTLOG", functioName);
                    return new IMtgaOutputLogPartResult[0];
                }

            if (skipped.Any(i => part.Contains(i)))
                return null;

            var ignoredTextKey = ignoredMatch.FirstOrDefault(i => part.Contains(i));
            if (ignoredTextKey != null)
                return new[] { new IgnoredMatchResult() { LogTextKey = ignoredTextKey } };

            ignoredTextKey = ignored.FirstOrDefault(i => part.Contains(i));
            if (ignoredTextKey != null)
                return new[] { new IgnoredResult() { LogTextKey = ignoredTextKey } };

            return new[] { new UnknownResult() };
        }
    }
}
