using System.Collections.Generic;
using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger
{
    public class GetDecksListResult : MtgaOutputLogPartResultBase<PayloadRaw<ICollection<CourseDeckRaw>>>
    {
    }
}