using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.DraftPickStatus;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class DraftStatusConverter : GenericConverter<DraftPickStatusResult, PayloadRaw<string>>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== BotDraft_DraftStatus";
    }
}