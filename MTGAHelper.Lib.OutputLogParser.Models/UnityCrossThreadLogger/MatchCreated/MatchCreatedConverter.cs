using System.Runtime.CompilerServices;
using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class MatchCreatedConverter : GenericConverter<MatchCreatedResult, PayloadRaw<MatchCreatedRaw>>
    {
        protected override MatchCreatedResult CreateT(PayloadRaw<MatchCreatedRaw> raw)
        {
            return new MatchCreatedResult { Raw = raw, MatchId = raw.payload.matchId };
        }
    }
}
