
namespace MTGAHelper.Entity.OutputLogParsing
{
    public class DuelSceneSideboardingStopRaw
    {
        public string jsonrpc { get; set; }
        public string method { get; set; }
        public Params<DuelSceneSideboardingStopPayloadObjectRaw> @params { get; set; }
        public string id { get; set; }
    }

    //public interface IMatchPayloadObject : ITagPayloadObject
    //{
    //    string playerId { get; set; }
    //    int gameNumber { get; set; }
    //    string matchId { get; set; }
    //    string eventId { get; set; }
    //}

    public class DuelSceneSideboardingStopPayloadObjectRaw// : IMatchPayloadObject
    {
        public string playerId { get; set; }
        public int gameNumber { get; set; }
        public string matchId { get; set; }
        public string eventId { get; set; }
    }

    //public class Params
    //{
    //    public string messageName { get; set; }
    //    public string humanContext { get; set; }
    //    public DuelSceneSideboardingStopPayloadObjectRaw payloadObject { get; set; }
    //    public string transactionId { get; set; }
    //}
}
