using System.Collections.Generic;
using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger
{
    public class GetPlayerCardsResult : MtgaOutputLogPartResultBase<PayloadRaw<Dictionary<int, int>>>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.GetPlayerCards;

        //public DateTime Date { get; set; }
        //public Dictionary<int, int> Cards { get; set; }
    }
}
