using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.MakeHumanDraftPick;

namespace MTGAHelper.Lib.OutputLogParser.Readers.RequestToServer
{
    public class MakeHumanDraftPickConverter : GenericConverter<MakeHumanDraftPickResult, RequestRaw<MakeHumanDraftPickRaw>>, IMessageReaderRequestToServer
    {
        public override string LogTextKey => "==> Draft.MakeHumanDraftPick";
    }
}