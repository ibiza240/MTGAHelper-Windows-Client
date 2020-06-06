using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MTGAHelper.Lib.OutputLogParser.Models;

namespace MTGAHelper.Lib.OutputLogParser
{
    public class ReaderAccountsClient : IReaderMtgaOutputLogPart
    {
        public string LogTextKey => "logged in to account:";

        readonly Regex regexPlayerName = new Regex(@"logged in to account: (.*?#\d{5})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ICollection<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            var regexResult = regexPlayerName.Match(part);
            if (regexResult.Success)
            {
                return new[] { new PlayerNameResult {
                    Name = regexResult.Groups[1].Value,
                    LogTextKey = LogTextKey,
                }};
            }
            else
                return new IMtgaOutputLogPartResult[0];
        }
    }

    public class ReaderAccountsAccountClient : IReaderMtgaOutputLogPart
    {
        public string LogTextKey => ", AccountID:";

        readonly Regex regexPlayerName = new Regex(@"DisplayName:(.*?#\d{5}), AccountID:([A-Z0-9]+),", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ICollection<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            var regexResult = regexPlayerName.Match(part);
            if (regexResult.Success)
            {
                return new[] { new PlayerNameResult {
                    Name = regexResult.Groups[1].Value,
                    AccountId = regexResult.Groups[2].Value,
                    LogTextKey = LogTextKey,
                }};
            }
            else
            {
                Debugger.Break();
                return new IMtgaOutputLogPartResult[0];
            }
        }
    }
}
