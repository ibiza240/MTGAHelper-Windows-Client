using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger
{
    public class EventClaimPrizeResult : MtgaOutputLogPartResultBase<PayloadRaw<EventClaimPrizeRaw>>//, ITagMatchResult//, IMtgaOutputLogPartResult<DeckSubmitRaw>
    {
    }
}
