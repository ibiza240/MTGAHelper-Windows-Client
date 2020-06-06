using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.SubmitDeckReq;
using MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.GreMessageType
{
    public class SubmitDeckReqConverter : GenericConverter<SubmitDeckReqResult, SubmitDeckReqRaw>
    {
        public override string LogTextKey => ReaderMtgaOutputLogGreMatchToClient.GREMessageType_SubmitDeckReq;
    }
}
