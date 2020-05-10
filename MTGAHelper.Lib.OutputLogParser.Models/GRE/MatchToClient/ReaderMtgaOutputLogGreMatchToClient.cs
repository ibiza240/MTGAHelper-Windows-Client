using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE
{
    public class ReaderMtgaOutputLogGreMatchToClient : IReaderMtgaOutputLogJsonMulti
    {
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
            ConnectRespConverter converterConnectResp,
            MulliganReqConverter converterMulliganReq,
            GameStateMessageConverter converterGameStateMessage,
            SubmitDeckReqConverter converterSubmitDeckAfterGame1,
            DieRollResultsRespConverter dieRollResultsRespConverter,
            SelectNReqConverter selectNReqConverter,
            GroupReqConverter groupReqConverter
        )
        {
            converters.Add(GREMessageType_IntermissionReq, converterIntermissionReq);
            converters.Add(GREMessageType_ConnectResp, converterConnectResp);
            converters.Add(GREMessageType_MulliganReq, converterMulliganReq);
            converters.Add(GREMessageType_GameStateMessage, converterGameStateMessage);
            converters.Add(GREMessageType_QueuedGameStateMessage, converterGameStateMessage);
            converters.Add(GREMessageType_SubmitDeckReq, converterSubmitDeckAfterGame1);
            converters.Add(GREMessageType_DieRollResultsResp, dieRollResultsRespConverter);
            converters.Add(GREMessageType_SelectNReq, selectNReqConverter);
            converters.Add(GREMessageType_GroupReq, groupReqConverter);
        }

        public ICollection<IMtgaOutputLogPartResult> ParseJsonMulti(string json)
        {
            var raw = JsonConvert.DeserializeObject<GreMatchToClientMessageRoot>(json);

            if (raw.greToClientEvent != null)
                return ParseGreToClientEvent(raw.timestamp, raw.greToClientEvent);

            else if (raw.matchGameRoomStateChangedEvent != null)
                return ParseMatchGameRoomStateChangedEvent(raw.timestamp, raw.matchGameRoomStateChangedEvent)
                    .Cast<IMtgaOutputLogPartResult>()
                    .ToArray();

            else if (json.Contains("MatchServiceErrorCode_UnknownServerError"))
                return new[] { new IgnoredMatchResult() };

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
                IMtgaOutputLogPartResult result = null;
                string type = submsg.type;
                var subjson = JsonConvert.SerializeObject(submsg);

                if (converters.ContainsKey(type))
                {
                    //try
                    //{
                    result = converters[type].ParseJson(subjson);
                    result.Timestamp = timestamp;
                    result.SubPart = subjson;
                    //}
                    //catch (Exception ex)
                    //{
                    //    System.Diagnostics.Debugger.Break();
                    //}
                }
                else if (IgnoredTypes.Contains(type))
                {
                    result = new IgnoredMatchResult(timestamp) { SubPart = subjson };
                }
                else
                {
                    result = new UnknownMatchResult(timestamp) { SubPart = subjson };
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
            } };
        }
    }
}
