using MTGAHelper.Entity.OutputLogParsing;
using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class GetPlayerCardsResult : MtgaOutputLogPartResultBase<PayloadRaw<Dictionary<int, int>>>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.GetPlayerCards;

        //public DateTime Date { get; set; }
        //public Dictionary<int, int> Cards { get; set; }
    }
}
