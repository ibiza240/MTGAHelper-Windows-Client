using System.Collections.Generic;
using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class GetPlayerQuestsConverter : GenericConverter<GetPlayerQuestsResult, PayloadRaw<ICollection<GetPlayerQuestRaw>>>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Quest.GetPlayerQuests";
    }
}
