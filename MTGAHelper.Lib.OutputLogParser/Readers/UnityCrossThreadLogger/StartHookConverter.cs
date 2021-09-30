using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.StartHook;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class StartHookConverter : GenericConverter<StartHookResult, StartHookRaw>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== StartHook";
    }
}