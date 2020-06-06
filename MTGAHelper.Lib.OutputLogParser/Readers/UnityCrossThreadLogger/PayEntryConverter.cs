using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class PayEntryConverter : GenericConverter<PayEntryResult, PayloadRaw<PayEntryRaw>>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Event.PayEntry";
    }
}
