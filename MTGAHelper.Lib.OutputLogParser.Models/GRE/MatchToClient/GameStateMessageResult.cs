using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.GameStateMessage;

namespace MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient
{
    public class GameStateMessageResult : MtgaOutputLogPartResultBase<GameStateMessageRaw>, ITagMatchResult//, IMtgaOutputLogPartResult<GameStateMessageRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.GameStateMessage;

        //public new GameStateMessageRaw Raw { get; set; }
    }

}
