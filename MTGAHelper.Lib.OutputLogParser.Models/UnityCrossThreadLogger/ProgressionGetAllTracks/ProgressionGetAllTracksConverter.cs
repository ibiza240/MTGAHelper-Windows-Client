using MTGAHelper.Entity.OutputLogParsing;
using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class ProgressionGetAllTracksConverter : GenericConverter<ProgressionGetAllTracksResult, PayloadRaw<ICollection<ProgressionGetAllTracksRaw>>>
    {
    }
}
