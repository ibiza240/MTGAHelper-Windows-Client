using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger
{
    public class MatchCreatedResult : MtgaOutputLogPartResultBase<PayloadRaw<MatchCreatedRaw>>, ITagMatchResult//, IMtgaOutputLogPartResult<MatchCreatedRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.MatchCreated;

        //public new MatchCreatedRaw Raw { get; set; }
    }
}
