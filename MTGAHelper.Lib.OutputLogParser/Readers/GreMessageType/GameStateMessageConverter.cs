using MTGAHelper.Lib.OutputLogParser.Models;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient;
using MTGAHelper.Lib.OutputLogParser.Models.GRE.MatchToClient.GameStateMessage;
using MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger;
using Newtonsoft.Json;

namespace MTGAHelper.Lib.OutputLogParser.Readers.GreMessageType
{
    public class GameStateMessageConverter : GenericConverter<GameStateMessageResult, GameStateMessageRaw>
    {
        public override string LogTextKey => ReaderMtgaOutputLogGreMatchToClient.GREMessageType_GameStateMessage;

        public override IMtgaOutputLogPartResult ParseJson(string json)
        {
            var raw = JsonConvert.DeserializeObject<GameStateMessageRaw>(json);
            var result = new GameStateMessageResult
            {
                MatchId = raw.gameStateMessage.gameInfo?.matchID,
                Raw = raw,
                LogTextKey = LogTextKey,
            };
            return result;
        }
    }

    public class QueuedGameStateMessageConverter : GameStateMessageConverter
    {
        public override string LogTextKey => ReaderMtgaOutputLogGreMatchToClient.GREMessageType_QueuedGameStateMessage;
    }
}
