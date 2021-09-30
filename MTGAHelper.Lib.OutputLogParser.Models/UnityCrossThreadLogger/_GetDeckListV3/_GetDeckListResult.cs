using System.Collections.Generic;
using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger._GetDeckListV3
{
    public class GetDecksListResult : MtgaOutputLogPartResultBase<PayloadRaw<ICollection<CourseDeckRaw>>>
    {
    }
}