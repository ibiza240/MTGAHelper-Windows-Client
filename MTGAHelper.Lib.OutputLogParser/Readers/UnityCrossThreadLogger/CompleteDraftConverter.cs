using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class CompleteDraftConverter : GenericConverter<CompleteDraftResult, PayloadRaw<CompleteDraftRaw>>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Event.CompleteDraft";
    }
}
