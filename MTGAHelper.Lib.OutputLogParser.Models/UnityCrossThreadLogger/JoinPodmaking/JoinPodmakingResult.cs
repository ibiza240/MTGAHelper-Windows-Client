using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class JoinPodmakingResult : MtgaOutputLogPartResultBase<JoinPodmakingRaw>, ITagMatchResult//, IMtgaOutputLogPartResult<MatchCreatedRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.MatchCreated;

        //public new MatchCreatedRaw Raw { get; set; }
    }
}
