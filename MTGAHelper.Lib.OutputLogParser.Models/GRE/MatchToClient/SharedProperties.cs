using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient
{
    public class Prompt
    {
        public int promptId { get; set; }
        public List<Parameter> parameters { get; set; }
    }

    public class Parameter
    {
        public string parameterName { get; set; }
        public string type { get; set; }
        public int numberValue { get; set; }
    }
}
