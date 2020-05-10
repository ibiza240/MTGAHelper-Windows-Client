using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient.MatchGameRoomStateChanged.Raw
{
    public class GameRoomConfig
    {
        public string eventId { get; set; }
        public List<ReservedPlayer> reservedPlayers { get; set; }
        public string matchId { get; set; }
        //public MatchConfig matchConfig { get; set; }
        //public GreConfig greConfig { get; set; }
        //public string greHostLoggerLevel { get; set; }
        //public int joinRoomTimeoutSecs { get; set; }
        //public int playerDisconnectTimeoutSecs { get; set; }
    }

    public class ResultList
    {
        public string scope { get; set; }
        public string result { get; set; }
        public int winningTeamId { get; set; }
    }

    public class FinalMatchResultRaw
    {
        public string matchId { get; set; }
        public string matchCompletedReason { get; set; }
        public List<ResultList> resultList { get; set; }
    }

    public class GameRoomInfo
    {
        public List<Player> players { get; set; }
        public GameRoomConfig gameRoomConfig { get; set; }
        public string stateType { get; set; }
        public FinalMatchResultRaw finalMatchResult { get; set; }
    }

    public class ConnectionInfo
    {
        public string connectionState { get; set; }
    }

    public class ReservedPlayer
    {
        public string userId { get; set; }
        public string playerName { get; set; }
        public int systemSeatId { get; set; }
        public int teamId { get; set; }
        public ConnectionInfo connectionInfo { get; set; }
        public string courseId { get; set; }
    }

    public class Player
    {
        public string userId { get; set; }
        public int systemSeatId { get; set; }
    }
}
