using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient.ConnectResp.Raw;
using System;
using System.Collections.Generic;
using System.Text;

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
