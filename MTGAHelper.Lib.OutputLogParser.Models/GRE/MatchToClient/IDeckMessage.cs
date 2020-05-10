using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient
{
    public interface IDeckMessage
    {
        List<int> deckCards { get; }
        List<int> sideboardCards { get; }
    }
}
