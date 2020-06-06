using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.IntermissionReq;
using MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.GreMessageType
{
    public class IntermissionReqConverter : GenericConverter<IntermissionReqResult, IntermissionReqRaw>
    {
        public override string LogTextKey => ReaderMtgaOutputLogGreMatchToClient.GREMessageType_IntermissionReq;
    }
}
