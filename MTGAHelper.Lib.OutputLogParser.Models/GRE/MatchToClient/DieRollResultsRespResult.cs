using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.DieRollResultsResp;

namespace MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient
{
    public class DieRollResultsRespResult : MtgaOutputLogPartResultBase<DieRollResultsRespRaw>, ITagMatchResult//, IMtgaOutputLogPartResult<ConnectRespRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.ConnectResp;

        //public new ConnectRespRaw Raw { get; set; }
    }
}
