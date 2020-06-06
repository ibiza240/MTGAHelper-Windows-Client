using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.ConnectResp;

namespace MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient
{
    public class ConnectRespResult : MtgaOutputLogPartResultBase<ConnectRespRaw>, ITagMatchResult//, IMtgaOutputLogPartResult<ConnectRespRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.ConnectResp;

        //public new ConnectRespRaw Raw { get; set; }
    }
}
