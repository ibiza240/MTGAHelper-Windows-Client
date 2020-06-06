namespace MTGAHelper.Lib.OutputLogParser.Models.GRE.ClientToMatch
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
