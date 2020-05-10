using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient.IntermissionReq.Raw
{
    public class IntermissionReqRaw : GreMatchToClientSubMessageBase
    {
        public IntermissionReq intermissionReq { get; set; }
    }

    public class OptionPrompt
    {
        public int promptId { get; set; }
    }

    public class Option
    {
        public OptionPrompt optionPrompt { get; set; }
        public string responseType { get; set; }
    }

    public class Parameter
    {
        public string parameterName { get; set; }
        public string type { get; set; }
        public int numberValue { get; set; }
    }

    public class IntermissionPrompt
    {
        public int promptId { get; set; }
        public List<Parameter> parameters { get; set; }
    }

    public class Result
    {
        public string scope { get; set; }
        public string result { get; set; }
        public int winningTeamId { get; set; }
        public string reason { get; set; }
    }

    public class IntermissionReq
    {
        public List<Option> options { get; set; }
        public IntermissionPrompt intermissionPrompt { get; set; }
        public string gameResultType { get; set; }
        public int winningTeamId { get; set; }
        public Result result { get; set; }
    }
}
