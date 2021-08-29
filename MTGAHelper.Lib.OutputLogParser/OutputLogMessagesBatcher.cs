using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MTGAHelper.Lib.OutputLogParser.Models;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

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
                try
                {
                    //if (m.Part.Contains("==> LogBusinessEvents"))
                    //    Debugger.Break();

                    //if (m.Part.Contains("\"duration\": 12.896253999999999,")) System.Diagnostics.Debugger.Break();

                    var matchDisconnected = m is StateChangedResult stateChanged && stateChanged.SignifiesMatchEnd;
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
                    //if (m is LogInfoRequestResult logRequestContainer)
                    //{
                    //    var logRequest = logRequestContainer.RequestParams;
                    //    logRequestEndOfMatch = (logRequest.humanContext.Contains("during a match") || logRequest.humanContext == "End of match report");
                    //}

                    if (matchDisconnected || logRequestEndOfMatch)
                    {
                        currentMatchId = $"Lobby-{lastMatchId}";
                    }
                    //else if (m is MatchCreatedResult matchCreated)
                    else if (m is ConnectingToMatchIdResult matchCreated)
                    {
                        currentMatchId = matchCreated.MatchId;
                        lastMatchId = currentMatchId;
                    }

                    var isLastMatch = m is ITagMatchResult;
                    //||
                    //(
                    //    m is LogInfoRequestResult log &&
                    //    //log.Raw.@params.payloadObject.matchId == lastMatchId
                    //    log.Raw.request.Contains(lastMatchId)
                    //);

                    var matchId = isLastMatch ? lastMatchId : currentMatchId;

                    msgsWithMatch.Add(new MessageWithMatch(m, matchId));
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                }
            }

            //var test = msgsWithMatch.Where(i => i?.MatchId == null).ToArra

            return msgsWithMatch
                .GroupBy(i => i.MatchId)
                .ToDictionary(i => i.Key, i => i.Select(x => x.Message).ToArray());
        }
    }
}