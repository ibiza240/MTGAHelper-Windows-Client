using System.Collections.Generic;
using MTGAHelper.Lib.OutputLogParser.Models;

namespace MTGAHelper.Lib.OutputLogParser.Readers
{
    public interface ILogMessageReader
    {
        string LogTextKey { get; }
        bool DoesParse(string part);
        IEnumerable<IMtgaOutputLogPartResult> ParsePart(string part);
    }
}