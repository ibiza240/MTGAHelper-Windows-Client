using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient.GameStateMessage.Raw;
using Newtonsoft.Json;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient
{
    public class GameStateMessageConverter : GenericConverter<GameStateMessageResult, GameStateMessageRaw>
    {
        public override IMtgaOutputLogPartResult ParseJson(string json)
        {
            var raw = JsonConvert.DeserializeObject<GameStateMessageRaw>(json);
            var result = new GameStateMessageResult
            {
                MatchId = raw.gameStateMessage.gameInfo?.matchID,
                Raw = raw,
            };
            return result;
        }
    }
}
