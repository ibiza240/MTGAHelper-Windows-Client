using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity.OutputLogParsing
{
    //public interface ITagPayloadObject
    //{
    //}

    public class DuelSceneSideboardingStartRaw
    {
        public string jsonrpc { get; set; }
        public string method { get; set; }
        public Params<DuelSceneSideboardingStartPayloadObjectRaw> @params { get; set; }
        public string id { get; set; }
    }

    public class DuelSceneSideboardingStartPayloadObjectRaw// : IMatchPayloadObject
    {
        public string playerId { get; set; }
        public int gameNumber { get; set; }
        public string matchId { get; set; }
        public string eventId { get; set; }
    }

    public class Params
    {
        public string messageName { get; set; }
        public string humanContext { get; set; }
        public dynamic payloadObject { get; set; }
        public string transactionId { get; set; }
    }

    public class Params<T>
    {
        public string messageName { get; set; }
        public string humanContext { get; set; }
        public T payloadObject { get; set; }
        public string transactionId { get; set; }
    }
}
