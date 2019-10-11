using System;
using System.Collections.Generic;
using System.Text;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;

namespace MTGAHelper.Entity.MtgaDeckStats
{
    public class MtgaDeckSummary
    {
        public string DeckId { get; set; }
        public string DeckImage { get; set; }
        public string DeckName { get; set; }
        public DateTime FirstPlayed { get; set; }
        public DateTime LastPlayed { get; set; }
        public float WinRate { get; set; }
        public string WinRateFormat { get; set; }
        public int WinRateNbWin { get; set; }
        public int WinRateNbLoss { get; set; }
        public int WinRateNbOther { get; set; }

        //public ConfigModelRawDeck DeckUsed { get; set; }
        public string DeckColor { get; set; }
    }

    public class MtgaDeckDetail
    {
        public string DeckId { get; set; }
        public string DeckImage { get; set; }
        public string DeckName { get; set; }
        //public DateTime FirstPlayed { get; set; }
        //public DateTime LastPlayed { get; set; }
        public ICollection<MtgaDeckStatsByFormat> StatsByFormat { get; set; }
        public ICollection<MatchResult> Matches { get; set; }

        //public ConfigModelRawDeck DeckUsed { get; set; }
        public string DeckColor { get; set; }
        public Dictionary<int, int> CardsMain { get; set; }
        public Dictionary<int, int> CardsSideboard { get; set; }
    }

    public class MtgaDeckAnalysis
    {
        public string DeckId { get; set; }
        public string DeckImage { get; set; }
        public string DeckName { get; set; }
        public ICollection<MtgaDeckAnalysisMatchInfo> MatchesInfo { get; set; }
    }

    public class MtgaDeckStatsByFormat
    {
        public string Format { get; set; }
        public int NbWins { get; set; }
        public int NbLosses { get; set; }
    }

    public class MtgaDeckAnalysisMatchInfo
    {
        public DateTime StartDateTime { get; set; }
        public string EventName { get; set; }
        public string OpponentColors { get; set; }
        public string OpponentRank { get; set; }
        public FirstTurnEnum FirstTurn { get; set; }
        public GameOutcomeEnum Outcome { get; set; }
        public int Mulligans { get; set; }
        public int OpponentMulligans { get; set; }
    }
}
