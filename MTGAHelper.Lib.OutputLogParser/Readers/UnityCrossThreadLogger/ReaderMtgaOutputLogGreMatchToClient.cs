using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MTGAHelper.Lib.OutputLogParser.Models;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient;
using MTGAHelper.Lib.OutputLogParser.Readers.GreMessageType;
using Newtonsoft.Json;
using Serilog;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class ReaderMtgaOutputLogGreMatchToClient : IMessageReaderUnityCrossThreadLogger
    {
        public string LogTextKey => "Match to ";

        public bool DoesParse(string part)
        {
            return part.StartsWith(LogTextKey);
        }

        public bool IsJson => true;

        readonly Dictionary<string, IReaderMtgaOutputLogJson> converters = new Dictionary<string, IReaderMtgaOutputLogJson>();

        public const string GREMessageType_IntermissionReq = "GREMessageType_IntermissionReq";
        public const string GREMessageType_ConnectResp = "GREMessageType_ConnectResp";
        public const string GREMessageType_MulliganReq = "GREMessageType_MulliganReq";
        public const string GREMessageType_GameStateMessage = "GREMessageType_GameStateMessage";
        public const string GREMessageType_QueuedGameStateMessage = "GREMessageType_QueuedGameStateMessage";
        public const string GREMessageType_SubmitDeckReq = "GREMessageType_SubmitDeckReq";
        public const string GREMessageType_DieRollResultsResp = "GREMessageType_DieRollResultsResp";
        public const string GREMessageType_SelectNReq = "GREMessageType_SelectNReq";
        public const string GREMessageType_GroupReq = "GREMessageType_GroupReq";

        readonly string[] skippedGRE = new[]
        {
            "GREMessageType_UIMessage",
            "GREMessageType_PromptReq",
            "GREMessageType_GetSettingsResp",
            "GREMessageType_TimerStateMessage",
            "GREMessageType_ActionsAvailableReq",
            "GREMessageType_OptionalActionMessage",
        };

        public static readonly string[] IgnoredTypes = new[]
        {
            //"GREMessageType_DieRollResultsResp",
            "GREMessageType_SetSettingsResp",
            "GREMessageType_PromptReq",
            "GREMessageType_ChooseStartingPlayerReq",
            "GREMessageType_UIMessage",
            "GREMessageType_TimerStateMessage",
            "GREMessageType_ActionsAvailableReq",
            "GREMessageType_GetSettingsResp",
            "GREMessageType_SelectTargetsReq",
            "GREMessageType_SubmitTargetsResp",
            "GREMessageType_DeclareAttackersReq",
            "GREMessageType_OptionalActionMessage",
            "GREMessageType_SubmitAttackersResp",
            "GREMessageType_PayCostsReq",
            "GREMessageType_SubmitBlockersResp",
            "GREMessageType_DeclareBlockersReq",
            "GREMessageType_CastingTimeOptionsReq",
            "GREMessageType_SearchReq",
            "GREMessageType_AssignDamageReq",
            "GREMessageType_AssignDamageConfirmation",
            "GREMessageType_OrderDamageConfirmation",
            "GREMessageType_OrderCombatDamageReq",
        };

        public ReaderMtgaOutputLogGreMatchToClient(
            IntermissionReqConverter converterIntermissionReq,
            GreConnectRespConverter converterConnectResp,
            MulliganReqConverter converterMulliganReq,
            GameStateMessageConverter converterGameStateMessage,
            QueuedGameStateMessageConverter converterQueuedGameStateMessage,
            SubmitDeckReqConverter converterSubmitDeckAfterGame1,
            DieRollResultsRespConverter dieRollResultsRespConverter,
            SelectNReqConverter selectNReqConverter,
            GroupReqConverter groupReqConverter,
            AuthenticateResponseConverter authenticateResponseConverter
        )
        {
            converters.Add(GREMessageType_IntermissionReq, converterIntermissionReq);
            converters.Add(GREMessageType_ConnectResp, converterConnectResp);
            converters.Add(GREMessageType_MulliganReq, converterMulliganReq);
            converters.Add(GREMessageType_GameStateMessage, converterGameStateMessage);
            converters.Add(GREMessageType_QueuedGameStateMessage, converterQueuedGameStateMessage);
            converters.Add(GREMessageType_SubmitDeckReq, converterSubmitDeckAfterGame1);
            converters.Add(GREMessageType_DieRollResultsResp, dieRollResultsRespConverter);
            converters.Add(GREMessageType_SelectNReq, selectNReqConverter);
            converters.Add(GREMessageType_GroupReq, groupReqConverter);
            this.authenticateResponseConverter = authenticateResponseConverter;
        }

        readonly Regex regexMatchTo = new Regex(@"^Match to ([A-Z0-9]+): (\w+)", RegexOptions.Multiline);
        readonly AuthenticateResponseConverter authenticateResponseConverter;

        public IEnumerable<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            var match = regexMatchTo.Match(part);
            if (!match.Success)
            {
                Log.Warning($"Match to did not match expected pattern: {part}");
                return new[] { new UnknownResult() };
            }

            var playerId = match.Groups[1].Value;
            var messageType = match.Groups[2].Value;

            if (messageType == "AuthenticateResponse")
                return authenticateResponseConverter.ParsePart(part);

            var json = part.Split(new[] { Environment.NewLine }, StringSplitOptions.None)[1];
            return ParseJsonMulti(json);
        }

        public ICollection<IMtgaOutputLogPartResult> ParseJsonMulti(string json)
        {
            var raw = JsonConvert.DeserializeObject<GreMatchToClientMessageRoot>(json);

            if (raw.greToClientEvent != null)
                return ParseGreToClientEvent(raw.timestamp, raw.greToClientEvent);

            if (raw.matchGameRoomStateChangedEvent != null)
                return ParseMatchGameRoomStateChangedEvent(raw.timestamp, raw.matchGameRoomStateChangedEvent)
                    .Cast<IMtgaOutputLogPartResult>()
                    .ToArray();

            if (json.Contains("MatchServiceErrorCode_UnknownServerError"))
                return new[] { new IgnoredMatchResult() { LogTextKey = "MatchServiceErrorCode_UnknownServerError" } };

            //throw new Exception("Unknown GreMatchToClientMessageRoot");
            Log.Warning("{outputLogError}: {json}", "OUTPUTLOG Unknown GreMatchToClientMessageRoot", json);
            return new[] { new UnknownMatchResult() };
        }

        ICollection<IMtgaOutputLogPartResult> ParseGreToClientEvent(long timestamp, GreMatchToClientEvent raw)
        {
            //if (timestamp == 636899006015817784)
            //    System.Diagnostics.Debugger.Break();

            //if (raw.greToClientMessages.Any(i => i.gameStateId == 128))
            //    System.Diagnostics.Debugger.Break();

            string matchId = null;
            var results = new List<IMtgaOutputLogPartResult>();

            foreach (var submsg in raw.greToClientMessages)
            {
                IMtgaOutputLogPartResult result;
                string type = submsg.type;
                var subjson = JsonConvert.SerializeObject(submsg);

                if (converters.ContainsKey(type))
                {
                    //try
                    //{
                    result = converters[type].ParseJson(subjson);
                    result.Timestamp = timestamp;
                    result.SubPart = subjson;
                    result.LogTextKey = type;
                    //}
                    //catch (Exception ex)
                    //{
                    //    System.Diagnostics.Debugger.Break();
                    //}
                }
                else if (skippedGRE.Contains(type))
                {
                    continue;
                }
                else
                {
                    var ignoredTextKey = IgnoredTypes.FirstOrDefault(i => i == type);
                    result = ignoredTextKey != null ?
                        new IgnoredMatchResult(timestamp) { SubPart = subjson, LogTextKey = ignoredTextKey } :
                        (IMtgaOutputLogPartResult)new UnknownMatchResult(timestamp) { SubPart = subjson };
                }

                if (string.IsNullOrEmpty(result.MatchId) == false)
                    matchId = result.MatchId;

                results.Add(result);
            }

            foreach (var r in results)
                r.MatchId = matchId;

            return results;
        }

        ICollection<IMtgaOutputLogPartResult<MatchGameRoomStateChangedEvent>> ParseMatchGameRoomStateChangedEvent(long timestamp, MatchGameRoomStateChangedEvent raw)
        {
            return new[] { new MatchGameRoomStateChangedEventResult {
                MatchId = raw.gameRoomInfo.gameRoomConfig.matchId,
                Raw = raw,
                Timestamp = timestamp,
                LogTextKey = "**MatchGameRoomStateChangedEvent detected in json**",
            } };
        }
    }
}
