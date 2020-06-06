using MTGAHelper.Entity.MtgaOutputLog;

namespace MTGAHelper.Lib.OutputLogParser.Models.OutputLogProgress
{
    public class CardForTurn
    {
        public int Turn { get; set; }
        public PlayerEnum Player { get; set; }
        public CardForTurnEnum Action { get; set; }
        public int CardGrpId { get; set; }

        public override string ToString()
        {
            return $"{Player} {Action} card {CardGrpId} on turn {Turn}";
        }
    }
}
