using Newtonsoft.Json;
using System;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public class RequestRaw2<T>
    {
        public string id { get; set; }
        public string request { get; set; }

        public T FetchPayload() => JsonConvert.DeserializeObject<T>(JsonConvert.DeserializeObject<ParamsRaw2>(request).Payload);
    }

    public class ParamsRaw2
    {
        public string Type { get; set; }
        public Guid TransId { get; set; }
        public string Payload { get; set; }
    }
}