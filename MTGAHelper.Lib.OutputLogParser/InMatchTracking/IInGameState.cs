using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    public interface IInGameState
    {
        bool IsReset { get; }
        int MySeatId { get; }
        int OpponentSeatId { get; }
        string OpponentScreenName { get; }
        int PriorityPlayer { get; }
        bool IsSideboarding { get; }

        IReadOnlyCollection<CardDrawInfo> OpponentCardsSeen { get; }
        IReadOnlyCollection<CardDrawInfo> MyLibrary { get; }
        IReadOnlyCollection<CardDrawInfo> MySideboard { get; }
    }
}