using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.StateChanged;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class StateChangedConverter : GenericConverter<StateChangedResult, StateChangedRaw>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "STATE CHANGED";
    }
}