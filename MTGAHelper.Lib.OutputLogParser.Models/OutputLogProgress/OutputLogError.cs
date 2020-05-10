using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MTGAHelper.Lib.OutputLogProgress
{
    public enum OutputLogErrorType
    {
        Unknown,
        UnknownZoneTransfer,
        PlayerZero,
        CardForTurnUnknown,
    }

    public class OutputLogError
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public OutputLogErrorType Type { get; set; }
        public string MatchId { get; set; }
        public long Timestamp { get; set; }

        public OutputLogError(OutputLogErrorType type, long timestamp)
        {
            Type = type;
            Timestamp = timestamp;
        }
    }
}
