
namespace MTGAHelper.Entity.OutputLogParsing
{
    public class ClientConnectedRaw
    {
        public string jsonrpc { get; set; }
        public string method { get; set; }
        public Params<ClientConnectedPayloadObjectRaw> @params { get; set; }
        public string id { get; set; }
    }

    public class ClientConnectedPayloadObjectRaw// : ITagPayloadObject
    {
        public string playerId { get; set; }
        public string screenName { get; set; }

        //etc.
    }

    //public class Params
    //{
    //    public string messageName { get; set; }
    //    public string humanContext { get; set; }
    //    public ClientConnectedPayloadObjectRaw payloadObject { get; set; }
    //    public string transactionId { get; set; }
    //}
}
