using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.DraftPickStatus;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class DraftPickStatusConverter : GenericConverter<DraftPickStatusResult, PayloadRaw<string>>, IMessageReaderUnityCrossThreadLogger
    {
        //public override string LogTextKey => "<== Draft.MakePick";
        public override string LogTextKey => "<== BotDraft_DraftPick";
    }
}