using System.Collections.Generic;
using MTGAHelper.Lib.OutputLogParser.Models;

namespace MTGAHelper.Lib.OutputLogParser
{
    public class ReaderDetailedLogs : IReaderMtgaOutputLogPart
    {
        public string LogTextKey => "DETAILED LOGS:";

        public ICollection<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            if (part.Contains("ENABLED"))
                return new[] { new DetailedLoggingResult(true) { LogTextKey = LogTextKey } };
            if (part.Contains("DISABLED"))
                return new[] { new DetailedLoggingResult(false) { LogTextKey = LogTextKey } };

            return new[] { new UnknownResult() { LogTextKey = LogTextKey } };
        }
    }
}
