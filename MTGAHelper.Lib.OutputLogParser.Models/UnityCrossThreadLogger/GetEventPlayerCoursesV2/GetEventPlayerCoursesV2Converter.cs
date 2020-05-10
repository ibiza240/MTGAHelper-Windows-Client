using MTGAHelper.Entity.OutputLogParsing;
using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class GetEventPlayerCoursesV2Converter : GenericConverter<GetEventPlayerCoursesV2Result, PayloadRaw<ICollection<GetEventPlayerCourseV2Raw>>>
    {
    }
}
