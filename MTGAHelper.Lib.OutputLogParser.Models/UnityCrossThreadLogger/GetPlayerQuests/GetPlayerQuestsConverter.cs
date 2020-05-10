using System.Collections.Generic;
using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    public class GetPlayerQuestsConverter : GenericConverter<GetPlayerQuestsResult, PayloadRaw<ICollection<GetPlayerQuestRaw>>>
    {
    }
}
