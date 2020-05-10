using System.Collections.Generic;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient.SelectNReq.Raw
{
    public class SelectNReqRaw : GreMatchToClientSubMessageBase
    {
        public Prompt prompt { get; set; }
        public SelectNReq selectNReq { get; set; }
        public Prompt nonDecisionPlayerPrompt { get; set; }
        public string allowCancel { get; set; }
    }

    public class SelectNReq
    {
        public int minSel { get; set; }
        public int maxSel { get; set; }
        public string context { get; set; }
        public string optionType { get; set; }
        public string optionContext { get; set; }
        public string listType { get; set; }
        public List<int> ids { get; set; }
        public int idx { get; set; }
        public Prompt prompt { get; set; }
        public string idType { get; set; }
        public List<int> unfilteredIds { get; set; }
        public int sourceId { get; set; }
        public string validationType { get; set; }
    }
}
