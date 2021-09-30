using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetSeasonAndRankDetail;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class GetSeasonAndRankDetailConverter : GenericConverter<GetSeasonAndRankDetailResult, PayloadRaw<GetSeasonAndRankDetailRaw>>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Rank_GetSeasonAndRankDetails";
    }
}