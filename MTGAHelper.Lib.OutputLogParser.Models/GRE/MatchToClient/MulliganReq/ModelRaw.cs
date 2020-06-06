using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.MulliganReq
{
    public class MulliganReqRaw : GreMatchToClientSubMessageBase
    {
        public MulPrompt prompt { get; set; }
        public MulNonDecisionPlayerPrompt nonDecisionPlayerPrompt { get; set; }
    }

    public class Choice
    {
        public int choiceId { get; set; }
        public int responseValue { get; set; }
    }

    public class MulPrompt
    {
        public int promptId { get; set; }
        public List<MulParameter> parameters { get; set; }
        public List<Choice> choices { get; set; }
    }

    public class Reference
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class MulParameter
    {
        public string parameterName { get; set; }
        public string type { get; set; }
        public Reference reference { get; set; }
        public int? numberValue { get; set; }
    }

    public class MulNonDecisionPlayerPrompt
    {
        public int promptId { get; set; }
        public List<MulParameter> parameters { get; set; }
    }
}
