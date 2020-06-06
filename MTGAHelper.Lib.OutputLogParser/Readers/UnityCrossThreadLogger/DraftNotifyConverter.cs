using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class DraftNotifyConverter : GenericConverter<DraftNotifyResult, DraftNotifyRaw>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "Draft.Notify";
    }
}
