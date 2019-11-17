using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public class CardsOpened
    {
        public int grpId { get; set; }
        public int goldAwarded { get; set; }
        public int gemsAwarded { get; set; }
        public string set { get; set; }
    }

    public class CrackBoosterRaw
    {
        public List<CardsOpened> cardsOpened { get; set; }
        public double totalVaultProgress { get; set; }
        public int wildCardTrackMoves { get; set; }
        public int wildCardTrackPosition { get; set; }
        public int wildCardTrackCommons { get; set; }
        public int wildCardTrackUnCommons { get; set; }
        public int wildCardTrackRares { get; set; }
        public int wildCardTrackMythics { get; set; }
    }
}
