using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.RequestToServer
{
    public class MakeHumanDraftPickConverter : GenericConverter<MakeHumanDraftPickResult, RequestRaw<MakeHumanDraftPickRaw>>, IMessageReaderRequestToServer
    {
        public override string LogTextKey => "==> Draft.MakeHumanDraftPick";
    }
}
