using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient.ConnectResp.Raw;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient
{
    public class ConnectRespResult : MtgaOutputLogPartResultBase<ConnectRespRaw>, ITagMatchResult//, IMtgaOutputLogPartResult<ConnectRespRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.ConnectResp;

        //public new ConnectRespRaw Raw { get; set; }
    }
}
