using System.Collections.Generic;
using MTGAHelper.Entity.MtgaOutputLog;

namespace MTGAHelper.Lib.OutputLogParser.InMatchTracking
{
    public interface IInGameState
    {
        bool IsReset { get; }
        int MySeatId { get; }
        int OpponentSeatId { get; }
        string OpponentScreenName { get; }
        int PriorityPlayer { get; }
        int TurnNumber { get; }
        public PlayerEnum OnThePlay { get; }
        bool IsSideboarding { get; }

        IReadOnlyCollection<CardDrawInfo> OpponentCardsSeen { get; }
        IReadOnlyCollection<CardDrawInfo> OpponentCardsPrevGames { get; }
        IReadOnlyCollection<CardDrawInfo> MyLibrary { get; }
        IReadOnlyCollection<CardDrawInfo> MySideboard { get; }
    }
}