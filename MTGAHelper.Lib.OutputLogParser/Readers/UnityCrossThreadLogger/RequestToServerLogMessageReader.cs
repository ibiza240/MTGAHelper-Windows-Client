using System;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Lib.OutputLogParser.Models;

namespace MTGAHelper.Lib.OutputLogParser.Readers.UnityCrossThreadLogger
{
    public class RequestToServerLogMessageReader : IMessageReaderUnityCrossThreadLogger
    {
        public string LogTextKey => "==>";

        private readonly IReadOnlyCollection<IMessageReaderRequestToServer> readers;

        public RequestToServerLogMessageReader(IEnumerable<IMessageReaderRequestToServer> readers)
        {
            this.readers = readers.ToArray();
        }

        public bool DoesParse(string part)
        {
            return part.StartsWith(LogTextKey);
        }

        public IEnumerable<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            var reader = readers.FirstOrDefault(r => r.DoesParse(part));
            return reader?.ParsePart(part) ?? 
                new[] { new IgnoredResultRequestToServer() { LogTextKey = $"**First chars: {part.Substring(0, Math.Min(40, part.Length)).Split('{')[0]}**" } };
        }
    }
}
