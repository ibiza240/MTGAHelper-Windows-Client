using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetPlayerProgress;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class GetPlayerProgressConverter : GenericConverter<GetPlayerProgressResult, PayloadRaw<GetPlayerProgressRaw>>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Progression.GetPlayerProgress";
    }
}