using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.ConnectResp;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class ConnectRespConverter : GenericConverter<ConnectRespResult, ConnectRespRaw>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "ConnectResp";
    }
}
