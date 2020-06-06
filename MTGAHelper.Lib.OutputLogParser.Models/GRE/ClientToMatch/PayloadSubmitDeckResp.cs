using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.ConnectResp;

namespace MTGAHelper.Lib.OutputLogParser.Models.GRE.ClientToMatch
{
    public class PayloadSubmitDeckResp
    {
        public SubmitDeckResp submitDeckResp { get; set; }
    }

    public class SubmitDeckResp
    {
        public DeckMessage deck { get; set; }
    }
}
