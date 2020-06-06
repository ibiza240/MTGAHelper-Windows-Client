using System.Collections.Generic;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.GroupReq;

namespace MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient
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
