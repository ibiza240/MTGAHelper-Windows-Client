using Newtonsoft.Json;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public class RequestRaw<T>
    {
        public string id { get; set; }
        public string request { get; set; }

        public T FetchParams() => JsonConvert.DeserializeObject<ParamsRaw<T>>(request).@params;
    }

    public class ParamsRaw<T>
    {
        public string jsonrpc { get; set; }
        public string method { get; set; }
        public T @params { get; set; }
    }
}