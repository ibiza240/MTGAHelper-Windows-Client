using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient.MulliganReq.Raw
{
    public class MulliganReqRaw : GreMatchToClientSubMessageBase
    {
        public Prompt prompt { get; set; }
        public NonDecisionPlayerPrompt nonDecisionPlayerPrompt { get; set; }
    }

    public class Choice
    {
        public int choiceId { get; set; }
        public int responseValue { get; set; }
    }

    public class Prompt
    {
        public int promptId { get; set; }
        public List<Parameter> parameters { get; set; }
        public List<Choice> choices { get; set; }
    }

    public class Reference
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Parameter
    {
        public string parameterName { get; set; }
        public string type { get; set; }
        public Reference reference { get; set; }
        public int? numberValue { get; set; }
    }

    public class NonDecisionPlayerPrompt
    {
        public int promptId { get; set; }
        public List<Parameter> parameters { get; set; }
    }
}
