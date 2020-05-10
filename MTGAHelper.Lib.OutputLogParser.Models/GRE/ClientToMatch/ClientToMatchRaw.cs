using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.ClientToMatch.Raw
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
