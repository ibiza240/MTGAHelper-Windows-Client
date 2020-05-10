using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient.MatchGameRoomStateChanged.Raw;
using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient
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
