using System.Collections.Generic;
using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class GraphGetGraphStateConverter : GenericConverter<GraphGetGraphStateResult, GraphGetGraphStateRaw>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "<== Graph_GetGraphState";
    }
}