using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class GetCombinedRankInfoConverter : GenericConverter<GetCombinedRankInfoResult, PayloadRaw<GetCombinedRankInfoRaw>>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Event.GetCombinedRankInfo";
    }
}
