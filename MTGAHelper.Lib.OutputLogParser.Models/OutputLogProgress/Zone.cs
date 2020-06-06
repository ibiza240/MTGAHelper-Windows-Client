using MTGAHelper.Entity.MtgaOutputLog;

namespace MTGAHelper.Lib.OutputLogParser.Models.OutputLogProgress
{
    public class Zone
    {
        public Zone(OutputLogParser.Models.GRE.MatchToClient.GameStateMessage.Zone zone, int opponentSeatId, PlayerEnum playerFromInstanceId)
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
