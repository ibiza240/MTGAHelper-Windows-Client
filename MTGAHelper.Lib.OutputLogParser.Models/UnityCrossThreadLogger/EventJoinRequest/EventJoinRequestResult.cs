using System;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger
{
    public class EventJoinRequestResult : RequestParamsResult2<EventJoinRequestPayloadRaw>
    {
    }

    public partial class EventJoinRequestPayloadRaw
    {
        public string EventName { get; set; }
        public string EntryCurrencyType { get; set; }
        public long EntryCurrencyPaid { get; set; }
        public object CustomTokenId { get; set; }
    }
}