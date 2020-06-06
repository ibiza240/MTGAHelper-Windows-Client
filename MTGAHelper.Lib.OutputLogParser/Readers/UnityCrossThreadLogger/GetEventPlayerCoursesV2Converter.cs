using System.Collections.Generic;
using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class GetEventPlayerCoursesV2Converter : GenericConverter<GetEventPlayerCoursesV2Result, PayloadRaw<ICollection<GetEventPlayerCourseV2Raw>>>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Event.GetPlayerCoursesV2";
    }
}
