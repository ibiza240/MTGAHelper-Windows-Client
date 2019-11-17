using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public class DuelSceneGameStopRaw
    {
        public string jsonrpc { get; set; }
        public string method { get; set; }
        public Params<DuelSceneGameStopPayloadObjectRaw> @params { get; set; }
        public string id { get; set; }
    }

    public class DuelSceneGameStopPayloadObjectRaw// : IMatchPayloadObject
    {
        public string playerId { get; set; }
        public int seatId { get; set; }
        public int teamId { get; set; }
        public int gameNumber { get; set; }
        public string matchId { get; set; }
        public string eventId { get; set; }
        public int startingTeamId { get; set; }
        public int winningTeamId { get; set; }
        public string winningReason { get; set; }
        public List<object> mulliganedHands { get; set; }
        public int turnCount { get; set; }
        public int turnCountInFullControl { get; set; }
        public long secondsCount { get; set; }
        public long secondsCountInFullControl { get; set; }
        public int ropeShownCount { get; set; }
        public int ropeExpiredCount { get; set; }
    }

    //public class Params
    //{
    //    public string messageName { get; set; }
    //    public string humanContext { get; set; }
    //    public DuelSceneGameStopPayloadObjectRaw payloadObject { get; set; }
    //    public string transactionId { get; set; }
    //}
}
