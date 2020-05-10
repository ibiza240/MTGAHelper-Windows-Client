using System.Collections.Generic;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient.GroupReq.Raw;
using MTGAHelper.Lib.OutputLogParser.InMatchTracking;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient
{
    public class GroupReqResult : MtgaOutputLogPartResultBase<GroupReqRaw>, ITagMatchResult
    {
        public GroupReqResult(int seatId, AllowCancel allowCancel, IReadOnlyCollection<int> instanceIds, GroupType groupType, GroupingContext context, int maxToTopLibrary, int maxToBottomLibrary)
        {
            SeatId = seatId;
            AllowCancel = allowCancel;
            InstanceIds = instanceIds;
            GroupType = groupType;
            Context = context;
            MaxToTopLibrary = maxToTopLibrary;
            MaxToBottomLibrary = maxToBottomLibrary;
        }

        public int SeatId { get; }
        public AllowCancel AllowCancel { get; }
        public IReadOnlyCollection<int> InstanceIds { get; }
        public GroupType GroupType { get; }
        public GroupingContext Context { get; }
        public int MaxToTopLibrary { get; }
        public int MaxToBottomLibrary { get; }
    }
}
