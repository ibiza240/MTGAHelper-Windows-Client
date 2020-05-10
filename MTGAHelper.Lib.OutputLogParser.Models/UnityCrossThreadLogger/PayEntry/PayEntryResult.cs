using System.Collections.Generic;
using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class PayEntryResult : MtgaOutputLogPartResultBase<PayloadRaw<PayEntryRaw>>, IResultCardPool//, IMtgaOutputLogPartResult<RankUpdatedRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.RankUpdated;

        //public new RankUpdatedRaw Raw { get; set; }
        public List<int> CardPool => Raw.payload.CardPool;
    }

}
