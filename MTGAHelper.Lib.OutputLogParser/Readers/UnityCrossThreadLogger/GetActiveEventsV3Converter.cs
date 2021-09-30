using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetActiveEventsV3;
using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class GetActiveEventsV3Converter : GenericConverter<GetActiveEventsV3Result, PayloadRaw<ICollection<GetActiveEventsV3Raw>>>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Event.GetActiveEventsV3";
    }
}