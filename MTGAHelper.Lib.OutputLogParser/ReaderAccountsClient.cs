using System.Collections.Generic;
using System.Text.RegularExpressions;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using MTGAHelper.Lib.OutputLogParser.Models;

namespace MTGAHelper.Lib.OutputLogParser
{
    [AddMessageEvenIfDateNull]
    public class PlayerNameResult : MtgaOutputLogPartResultBase<string>, IMtgaOutputLogPartResult
    {
        public string Name { get; set; }
    }

    public class ReaderAccountsClient : IReaderMtgaOutputLogPart
    {
        readonly Regex regexPlayerName = new Regex(@"logged in to account: (.*?#\d{5})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ICollection<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            var regexResult = regexPlayerName.Match(part);
            if (regexResult.Success)
            {
                return new[] { new PlayerNameResult { Name = regexResult.Groups[1].Value } };
            }
            else
                return new IMtgaOutputLogPartResult[0];
        }
    }
}
