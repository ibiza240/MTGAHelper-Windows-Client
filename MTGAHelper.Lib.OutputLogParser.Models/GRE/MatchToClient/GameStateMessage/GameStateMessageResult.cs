using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient.GameStateMessage.Raw;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient
{
    public class GameStateMessageResult : MtgaOutputLogPartResultBase<GameStateMessageRaw>, ITagMatchResult//, IMtgaOutputLogPartResult<GameStateMessageRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.GameStateMessage;

        //public new GameStateMessageRaw Raw { get; set; }
    }

}
