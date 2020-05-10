using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class GetCombinedRankInfoResult : MtgaOutputLogPartResultBase<PayloadRaw<GetCombinedRankInfoRaw>>//, IMtgaOutputLogPartResult<GetCombinedRankInfoRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.GetCombinedRankInfo;

        //public new GetCombinedRankInfoRaw Raw { get; set; }
    }
}
