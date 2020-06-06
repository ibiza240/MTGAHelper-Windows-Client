using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.DieRollResultsResp;
using MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.GreMessageType
{
    public class DieRollResultsRespConverter : GenericConverter<DieRollResultsRespResult, DieRollResultsRespRaw>
    {
        public override string LogTextKey => ReaderMtgaOutputLogGreMatchToClient.GREMessageType_DieRollResultsResp;
    }
}
