using MTGAHelper.Entity.OutputLogParsing;
using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class CompleteDraftResult : MtgaOutputLogPartResultBase<PayloadRaw<CompleteDraftRaw>>, IResultCardPool//, IMtgaOutputLogPartResult<CompleteDraftRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.CompleteDraft;

        //public CompleteDraftRaw Raw { get; set; }
        public List<int> CardPool => Raw.payload.CardPool;
    }
}
