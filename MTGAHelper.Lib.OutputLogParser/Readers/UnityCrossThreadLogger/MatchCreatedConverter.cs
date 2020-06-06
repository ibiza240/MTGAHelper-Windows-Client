using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class MatchCreatedConverter : GenericConverter<MatchCreatedResult, PayloadRaw<MatchCreatedRaw>>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Event.MatchCreated";

        protected override MatchCreatedResult CreateT(PayloadRaw<MatchCreatedRaw> raw)
        {
            return new MatchCreatedResult {
                Raw = raw,
                MatchId = raw.payload.matchId,
                LogTextKey = LogTextKey,
            };
        }
    }
}
