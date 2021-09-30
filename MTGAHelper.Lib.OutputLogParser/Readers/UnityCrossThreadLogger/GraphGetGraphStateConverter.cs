using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GraphGetGraphState;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class GraphGetGraphStateConverter : GenericConverter<GraphGetGraphStateResult, GraphGetGraphStateRaw>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Graph_GetGraphState";
    }
}