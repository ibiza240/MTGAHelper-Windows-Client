using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.EventClaimPrize
{
    public class EventClaimPrizeResult : MtgaOutputLogPartResultBase<PayloadRaw<EventClaimPrizeRaw>>//, ITagMatchResult//, IMtgaOutputLogPartResult<DeckSubmitRaw>
    {
    }
}