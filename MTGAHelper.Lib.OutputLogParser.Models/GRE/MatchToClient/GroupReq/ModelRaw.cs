using System.Collections.Generic;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient.GroupReq.Raw
{
    public class GroupReqRaw : GreMatchToClientSubMessageBase
    {
        public Prompt prompt { get; set; }
        public GroupReq groupReq { get; set; }
        public Prompt nonDecisionPlayerPrompt { get; set; }
        public string allowCancel { get; set; }
    }

    public class GroupReq
    {
        public List<int> instanceIds { get; set; }
        public List<GroupSpec> groupSpecs { get; set; }
        public int totalSelected { get; set; }
        public string groupType { get; set; }
        public string context { get; set; }
    }

    public class GroupSpec
    {
        public int upperBound { get; set; }
        public string zoneType { get; set; }
        public string subZoneType { get; set; }
    }
}
