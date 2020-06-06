using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    // TODO: verify if this is correct LogTextKey
    public class MythicRatingUpdatedConverter : GenericConverter<MythicRatingUpdatedResult, PayloadRaw<MythicRatingUpdatedRaw>>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== MythicRating.Updated";
    }
}