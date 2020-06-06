using System.Collections.Generic;
using MTGAHelper.Lib.OutputLogParser.Models;

namespace MTGAHelper.Lib.OutputLogParser
{
    internal class ReaderIgnored : IReaderMtgaOutputLogPart, IReaderMtgaOutputLogJson<string>
    {
        public string LogTextKey => Constants.LOGTEXTKEY_UNKNOWN;

        public bool IsJson => false;

        public IMtgaOutputLogPartResult ParseJson(string json)
        {
            return new IgnoredResult();
        }

        public ICollection<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            return new[] { new IgnoredResult() };
        }
    }

    internal class ReaderIgnoredMatch : IReaderMtgaOutputLogPart, IReaderMtgaOutputLogJson<string>
    {
        public string LogTextKey => Constants.LOGTEXTKEY_UNKNOWN;

        public bool IsJson => false;

        public IMtgaOutputLogPartResult ParseJson(string json)
        {
            return new IgnoredMatchResult();
        }

        public ICollection<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            return new[] { new IgnoredMatchResult() };
        }
    }
}
