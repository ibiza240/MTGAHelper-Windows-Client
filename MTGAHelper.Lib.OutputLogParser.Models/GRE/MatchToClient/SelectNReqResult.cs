using System.Collections.Generic;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.SelectNReq;

namespace MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient
{
    public class SelectNReqResult : MtgaOutputLogPartResultBase<SelectNReqRaw>, ITagMatchResult
    {
        public SelectNReqResult(int seatId, AllowCancel allowCancel, IReadOnlyCollection<int> ids, OptionType optionType, IdType idType, int minTake, int maxTake)
        {
            SeatId = seatId;
            AllowCancel = allowCancel;
            Ids = ids;
            OptionType = optionType;
            IdType = idType;
            MinTake = minTake;
            MaxTake = maxTake;
        }

        public int SeatId { get; }
        public AllowCancel AllowCancel { get; }
        public IReadOnlyCollection<int> Ids { get; }
        public OptionType OptionType { get; }
        public IdType IdType { get; }
        public int MinTake { get; }
        public int MaxTake { get; }
    }
}
