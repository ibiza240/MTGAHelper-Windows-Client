using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class CompleteDraftConverter : GenericConverter<CompleteDraftResult, CompleteDraftRaw>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Draft_CompleteDraft";
    }
}