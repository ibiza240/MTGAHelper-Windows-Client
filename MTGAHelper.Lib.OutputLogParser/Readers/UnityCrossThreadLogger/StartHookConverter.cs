using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class StartHookConverter : GenericConverter<StartHookResult, StartHookRaw>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== StartHook";
    }
}