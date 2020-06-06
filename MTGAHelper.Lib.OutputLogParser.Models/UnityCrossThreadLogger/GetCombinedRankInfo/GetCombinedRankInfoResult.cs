using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger
{
    public class GetCombinedRankInfoResult : MtgaOutputLogPartResultBase<PayloadRaw<GetCombinedRankInfoRaw>>//, IMtgaOutputLogPartResult<GetCombinedRankInfoRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.GetCombinedRankInfo;

        //public new GetCombinedRankInfoRaw Raw { get; set; }
    }
}
