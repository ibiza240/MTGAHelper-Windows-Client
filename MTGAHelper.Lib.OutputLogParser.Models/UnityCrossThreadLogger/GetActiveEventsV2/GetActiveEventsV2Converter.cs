using MTGAHelper.Entity.OutputLogParsing;
using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class GetActiveEventsV2Converter : GenericConverter<GetActiveEventsV2Result, PayloadRaw<ICollection<GetActiveEventsV2Raw>>>
    {
    }
}
