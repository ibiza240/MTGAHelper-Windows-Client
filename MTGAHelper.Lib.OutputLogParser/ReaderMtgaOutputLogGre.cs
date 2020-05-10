using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.Exceptions;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE
{
    public interface IReaderMtgaOutputLogGreMatchToClient
    {
        IMtgaOutputLogPartResult Parse(string part, string messageType, string json);
    }

    public class ReaderMtgaOutputLogGre : ReaderMtgaOutputLogJsonParser, IReaderMtgaOutputLogPart
    {
        readonly Dictionary<string, IReaderMtgaOutputLogJsonBase> converters = new Dictionary<string, IReaderMtgaOutputLogJsonBase>();

        readonly string[] ignored = new[]
        {
            "AuthenticateResponse",
        };

        public ReaderMtgaOutputLogGre(
            ReaderMtgaOutputLogGreMatchToClient converterGreMatchToClient
        )
        {
            converters.Add("Match to ", converterGreMatchToClient);
            converters.Add(" to Match", new ReaderIgnoredMatch());
            //
        }

        public ICollection<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            if (ignored.Any(i => part.Contains(i)))
                return new[] { new IgnoredMatchResult() };

            (var converterKey, var startIndex) = GetConverter(part);
            var reader = converters[converterKey];

            try
            {
                //if (part.Contains("MatchGameRoomStateChangedEvent"))
                //    System.Diagnostics.Debugger.Break();

                // Special case because of message summarized
                var json = reader is ReaderMessageSummarized ? GetPartMessageSummarized(part, startIndex) : GetJson(part, startIndex);

                if (reader is IReaderMtgaOutputLogJsonMulti readerMulti)
                    return readerMulti.ParseJsonMulti(json);
                else if (reader is IReaderMtgaOutputLogJson readerSingle)
                    return new[] { readerSingle.ParseJson(json) };

                throw new InvalidOperationException("Converter not recognized");
            }
            catch (MtgaOutputLogInvalidJsonException)
            {
                if (part.Trim().EndsWith("GreToClientEvent"))
                    // Managed case where sometimes the part is meaningless, e.g.:
                    // [Client GRE]3/31/2019 10:17:21 AM: Match to U4UUFBZZARFKZKXG3YY46FWOHU: GreToClientEvent
                    return new[] { new IgnoredMatchResult() };
                else
                    throw;
            }
        }

        (string converterKey, int startIndex) GetConverter(string part)
        {
            foreach (var converter in converters)
            {
                var startIndex = GetPartTypeIndex(part, converter.Key);
                if (startIndex >= 0)
                    return (converter.Key, startIndex);
            }

            //System.Diagnostics.Debugger.Break();
            throw new MtgaOutputLogUnknownMessageMatchException(part);
        }
    }
}
