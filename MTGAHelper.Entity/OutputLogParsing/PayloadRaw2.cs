using Newtonsoft.Json;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public class PayloadRaw2<T>
    {
        public int id { get; set; }
        public string payload { get; set; }

        public T FetchPayload() => JsonConvert.DeserializeObject<T>(payload);
    }
}