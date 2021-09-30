using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.EventSetDeck;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class EventSetDeckConverter : GenericConverter<EventSetDeckResult, RequestRaw2<EventSetDeckRaw>>, IMessageReaderRequestToServer
    {
        public override string LogTextKey => "==> Event_SetDeck";
    }
}