using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.ClientToMatch.Raw;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.ClientToMatch
{
    public class ClientToMatchResultGeneric : MtgaOutputLogPartResultBase<ClientToMatchRawGeneric>, ITagMatchResult
    {
    }

    public class ClientToMatchResult<TPayload> : MtgaOutputLogPartResultBase<ClientToMatchRaw<TPayload>>, ITagMatchResult//, IMtgaOutputLogPartResult<GameStateMessageRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.GameStateMessage;

        //public new GameStateMessageRaw Raw { get; set; }
    }
}
