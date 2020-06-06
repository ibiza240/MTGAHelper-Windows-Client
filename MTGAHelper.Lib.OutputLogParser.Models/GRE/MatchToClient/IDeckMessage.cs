using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient
{
    public interface IDeckMessage
    {
        List<int> deckCards { get; }
        List<int> sideboardCards { get; }
    }
}
