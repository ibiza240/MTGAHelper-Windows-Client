using System.Collections.Generic;
using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger
{
    public class CompleteDraftResult : MtgaOutputLogPartResultBase<PayloadRaw<CompleteDraftRaw>>, IResultCardPool//, IMtgaOutputLogPartResult<CompleteDraftRaw>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.CompleteDraft;

        //public CompleteDraftRaw Raw { get; set; }
        public List<int> CardPool => Raw.payload.CardPool;
    }
}
