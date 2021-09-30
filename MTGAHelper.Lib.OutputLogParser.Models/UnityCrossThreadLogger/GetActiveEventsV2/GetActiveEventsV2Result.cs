using System.Collections.Generic;
using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetActiveEventsV2
{
    public class GetActiveEventsV2Result : MtgaOutputLogPartResultBase<PayloadRaw<ICollection<GetActiveEventsV2Raw>>>//, IMtgaOutputLogPartResult<ICollection<GetActiveEventsV2Raw>>
    {
        //public override ReaderMtgaOutputLogPartTypeEnum ResultType => ReaderMtgaOutputLogPartTypeEnum.GetActiveEvents;

        //public new ICollection<GetActiveEventsV2Raw> Raw { get; set; }
    }
}