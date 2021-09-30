using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetCombinedRankInfo;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class GetCombinedRankInfoConverter : GenericConverter<GetCombinedRankInfoResult, GetCombinedRankInfoRaw>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Rank_GetCombinedRankInfo";
    }
}