using MTGAHelper.Lib.OutputLogParser.Models;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.ConnectingToMatchId;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MTGAHelper.Lib.OutputLogParser.Readers
{
    public class ConnectingToMatchIdConverter : GenericConverter<ConnectingToMatchIdResult, string>, IMessageReaderUnityCrossThreadLogger
    {
        public override string LogTextKey => "Connecting to matchId";

        public static readonly Regex regexMatchId = new Regex("^Connecting to matchId (.*?)$", RegexOptions.Compiled);

        public override IEnumerable<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            return new[] { CreateT(part) };
        }

        protected override ConnectingToMatchIdResult CreateT(string raw)
        {
            return new ConnectingToMatchIdResult
            {
                Raw = raw,
                MatchId = regexMatchId.Match(raw).Groups[1].Value,
                LogTextKey = LogTextKey,
            };
        }
    }
}