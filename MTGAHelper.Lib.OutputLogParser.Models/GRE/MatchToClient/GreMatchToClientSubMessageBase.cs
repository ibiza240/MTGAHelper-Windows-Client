using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient
{
    public interface IGreMatchToClientMessage
    {
        string type { get; set; }
        ICollection<int> systemSeatIds { get; set; }
        int msgId { get; set; }
        int gameStateId { get; set; }
    }

    public class GreMatchToClientSubMessageBase : IGreMatchToClientMessage
    {
        public string type { get; set; }
        public ICollection<int> systemSeatIds { get; set; }
        public int msgId { get; set; }
        public int gameStateId { get; set; }

        // From GREMessageType_GameStateMessage
        //public List<Team> teams { get; set; }
        //public List<Player> players { get; set; }
        //public TurnInfo turnInfo { get; set; }
        //public List<Zone> zones { get; set; }
        //public List<Annotation> annotations { get; set; }
        //public List<int> diffDeletedInstanceIds { get; set; }
        //public List<Timer> timers { get; set; }
        //public string update { get; set; }
    }

    public class GameInfo
    {
        public string matchID { get; set; }
        public int gameNumber { get; set; }
        public string stage { get; set; }
        public string type { get; set; }
        public string variant { get; set; }
        public string matchState { get; set; }
        public string matchWinCondition { get; set; }
        public int maxTimeoutCount { get; set; }
        public int maxPipCount { get; set; }
        public int timeoutDurationSec { get; set; }
        public string superFormat { get; set; }
        public string mulliganType { get; set; }
    }
}
