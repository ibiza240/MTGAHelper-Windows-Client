using System.Collections.Generic;

namespace MTGAHelper.Lib
{
    public interface IPossibleDateFormats
    {
        IReadOnlyList<string> Formats { get; }
    }
}