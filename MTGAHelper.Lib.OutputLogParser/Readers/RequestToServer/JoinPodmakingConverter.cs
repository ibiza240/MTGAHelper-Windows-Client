using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.RequestToServer
{
    public class JoinPodmakingConverter : GenericConverter<JoinPodmakingResult, RequestRaw<JoinPodmakingRaw>>, IMessageReaderRequestToServer
    {
        public override string LogTextKey => "==> Event.JoinPodmaking";
    }
}
