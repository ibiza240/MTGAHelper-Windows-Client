using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.MulliganReq;
using MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.GreMessageType
{
    public class MulliganReqConverter : GenericConverter<MulliganReqResult, MulliganReqRaw>
    {
        public override string LogTextKey => ReaderMtgaOutputLogGreMatchToClient.GREMessageType_MulliganReq;
    }
}
