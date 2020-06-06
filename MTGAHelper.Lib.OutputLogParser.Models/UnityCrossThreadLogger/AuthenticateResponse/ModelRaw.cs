namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger
{
    public class AuthenticateResponseRaw
    {
        public string transactionId { get; set; }
        public int requestId { get; set; }
        public AuthenticateResponseRawDetails authenticateResponse { get; set; }
    }

    public class AuthenticateResponseRawDetails
    {
        public string clientId { get; set; }
        public string sessionId { get; set; }
        public string screenName { get; set; }
    }

}
