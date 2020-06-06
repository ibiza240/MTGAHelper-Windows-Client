using System.Collections.Generic;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.MatchGameRoomStateChanged;

namespace MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient
{
    public class GreMatchToClientMessageRoot
    {
        public string transactionId { get; set; }
        public long timestamp { get; set; }
        public GreMatchToClientEvent greToClientEvent { get; set; }
        public MatchGameRoomStateChangedEvent matchGameRoomStateChangedEvent { get; set; }
    }

    public class GreMatchToClientEvent
    {
        public ICollection<dynamic> greToClientMessages { get; set; }
    }

    public class MatchGameRoomStateChangedEvent
    {
        public GameRoomInfo gameRoomInfo { get; set; }
    }
}
