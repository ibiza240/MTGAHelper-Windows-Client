using System.Collections.Generic;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;

namespace MTGAHelper.Lib.OutputLogParser
{
    public class ReaderDetailedLogs : IReaderMtgaOutputLogPart
    {
        public ICollection<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            if (part.Contains("ENABLED"))
                return new[] { new DetailedLoggingResult(true) };
            if (part.Contains("DISABLED"))
                return new[] { new DetailedLoggingResult(false) };

            return new[] { new UnknownResult() };
        }
    }
}
