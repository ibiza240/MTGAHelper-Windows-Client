using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class RankUpdatedResult : MtgaOutputLogPartResultBase<PayloadRaw<RankUpdatedRaw>>//, IMtgaOutputLogPartResult<RankUpdatedRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.RankUpdated;

        //public new RankUpdatedRaw Raw { get; set; }
    }

}
