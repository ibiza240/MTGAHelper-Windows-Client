using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class DraftStatusConverter : GenericConverter<DraftStatusResult, PayloadRaw<DraftMakePickRaw>>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Draft.DraftStatus";
    }
}
