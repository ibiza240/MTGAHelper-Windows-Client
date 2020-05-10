using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog
{
    internal class ReaderIgnored : IReaderMtgaOutputLogPart, IReaderMtgaOutputLogJson<string>
    {
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
