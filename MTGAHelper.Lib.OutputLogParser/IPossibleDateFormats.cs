using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser
{
    public interface IPossibleDateFormats
    {
        IReadOnlyList<string> Formats { get; }
    }
}