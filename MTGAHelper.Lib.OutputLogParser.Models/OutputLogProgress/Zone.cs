using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;

namespace MTGAHelper.Lib.OutputLogProgress
{
    public class Zone
    {
        public Zone(IO.Reader.MtgaOutputLog.GRE.MatchToClient.GameStateMessage.Raw.Zone zone, int opponentSeatId, PlayerEnum playerFromInstanceId)
        {
            this.Name = zone.type;
            if (zone.ownerSeatId.HasValue)
                this.Player = zone.ownerSeatId == opponentSeatId ? PlayerEnum.Opponent : PlayerEnum.Me;
            else
                this.Player = playerFromInstanceId;
        }

        public string Name { get; set; }
        public PlayerEnum Player { get; set; }
    }
}
