using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger;
using Newtonsoft.Json;

namespace MTGAHelper.Lib.OutputLogParser
{
    public class MessageWithMatch
    {
        public string MatchId { get; set; }
        public IMtgaOutputLogPartResult Message { get; set; }

        public MessageWithMatch(IMtgaOutputLogPartResult msg, string matchId)
        {
            Message = msg;
            MatchId = matchId;
        }
    }

    public class OutputLogMessagesBatcher
    {
        public Dictionary<string, IMtgaOutputLogPartResult[]> BatchMessages(IEnumerable<IMtgaOutputLogPartResult> messages)
        {
            var currentMatchId = "Lobby";
            var lastMatchId = currentMatchId;

            var msgsWithMatch = new List<MessageWithMatch>();

            foreach (var m in messages)
            {
                //if (m.Part.Contains("\"duration\": 12.896253999999999,")) System.Diagnostics.Debugger.Break();

                var matchDisconnected = m is StateChangedResult stateChanged && (stateChanged.Raw.Contains("\"new\":8}"));
                bool logRequestEndOfMatch = false;
                //try
                //{
                //    var test = (LogInfoRequestResult)m;
                //    logRequestEndOfMatch = m is LogInfoRequestResult logRequest && (logRequest.Raw.@params.humanContext.Contains("during a match") || logRequest.Raw.@params.humanContext == "End of match report");
                //}
                //catch (Exception ex)
                //{
                //    System.Diagnostics.Debugger.Break();
                //}
                if (m is LogInfoRequestResult logRequestContainer)
                {
                    var logRequest = JsonConvert.DeserializeObject<LogInfoRequestInnerRaw>(logRequestContainer.Raw.request);
                    logRequestEndOfMatch = (logRequest.@params.humanContext.Contains("during a match") || logRequest.@params.humanContext == "End of match report");
                }

                if (matchDisconnected || logRequestEndOfMatch)
                {
                    currentMatchId = $"Lobby-{lastMatchId}";
                }
                else if (m is MatchCreatedResult matchCreated)
                {
                    currentMatchId = matchCreated.MatchId;
                    lastMatchId = currentMatchId;
                }

                var isLastMatch = m is ITagMatchResult ||
                    (
                        m is LogInfoRequestResult log &&
                        //log.Raw.@params.payloadObject.matchId == lastMatchId
                        log.Raw.request.Contains(lastMatchId)
                    );

                var matchId = isLastMatch ? lastMatchId : currentMatchId;

                msgsWithMatch.Add(new MessageWithMatch(m, matchId));
            }

            //var test = msgsWithMatch.Where(i => i?.MatchId == null).ToArray();

            return msgsWithMatch
                .GroupBy(i => i.MatchId)
                .ToDictionary(i => i.Key, i => i.Select(x => x.Message).ToArray());
        }


    }
}
