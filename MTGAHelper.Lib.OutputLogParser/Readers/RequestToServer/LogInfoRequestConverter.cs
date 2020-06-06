using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;

namespace MTGAHelper.Lib.OutputLogParser.Readers.RequestToServer
{
    public class LogInfoRequestConverter : GenericConverter<LogInfoRequestResult, RequestRaw<LogInfoRequestRaw>>, IMessageReaderRequestToServer
    {
        public override string LogTextKey => "==> Log";
        //readonly Regex regexMessageName = new Regex("\"messageName\": \"(.*?)\"", RegexOptions.Compiled);

        //protected override MtgaOutputLogPartResultBase<LogInfoRequestRaw> ParseJsonTyped(string json)
        //{
        //    //var messageName = regexMessageName.Match(json).Groups[1].Value;
        //    //switch (messageName)
        //    //{
        //    //case "Client.SceneChange":
        //    //    return new LogInfoRequestResult
        //    //    {
        //    //        Raw = JsonConvert.DeserializeObject<LogInfoRequestRaw>(json)
        //    //    };
        //    //case "DuelScene.EndOfMatchReport":
        //    return new LogInfoRequestResult
        //    {
        //        Raw = JsonConvert.DeserializeObject<LogInfoRequestRaw>(json)
        //    };
        //    //default:
        //    //    dynamic rawDefault = JObject.Parse(json);
        //    //    return new LogInfoRequestResult<dynamic>
        //    //    {
        //    //        Raw = new LogInfoRequestRaw<JObject>
        //    //        {
        //    //            id = rawDefault.id.Value,
        //    //            jsonrpc = rawDefault.jsonrpc.Value,
        //    //            method = rawDefault.method.Value,
        //    //            @params = new Params<JObject>
        //    //            {
        //    //                transactionId = rawDefault.@params.transactionId,
        //    //                humanContext = rawDefault.@params.humanContext,
        //    //                messageName = rawDefault.@params.messageName,
        //    //                payloadObject = rawDefault.@params.payloadObject,
        //    //            }
        //    //        }
        //    //    };
        //    //}

        //    //return null;
        //}
    }
}
