using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.GRE.ClientToMatch
{
    public class ClientToMatchRawGeneric
    {
        public int requestId { get; set; }
        public string clientToMatchServiceMessageType { get; set; }
        public string timestamp { get; set; }
        public string transactionId { get; set; }
        public dynamic payload { get; set; }
    }

    public class ClientToMatchRaw<T> : PayloadRaw<T>
    {
        public int requestId { get; set; }
        public string clientToMatchServiceMessageType { get; set; }
        public string timestamp { get; set; }
        public string transactionId { get; set; }
    }
}
