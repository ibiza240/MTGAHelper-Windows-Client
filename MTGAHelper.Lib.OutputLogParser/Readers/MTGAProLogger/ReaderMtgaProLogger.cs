using MTGAHelper.Lib.OutputLogParser.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.OutputLogParser.Readers.MTGAProLogger
{
    public class ReaderMtgaProLogger : IReaderMtgaOutputLogPart
    {
        private readonly IEnumerable<IMessageReaderMtgaProLogger> readers;

        public string LogTextKey => throw new System.NotImplementedException();

        public ReaderMtgaProLogger(
        IEnumerable<IMessageReaderMtgaProLogger> readers
        )
        {
            this.readers = readers;
        }

        public ICollection<IMtgaOutputLogPartResult> ParsePart(string part)
        {
            var reader = readers.FirstOrDefault(c => c.DoesParse(part));
            if (reader != null)
                try
                {
                    return reader.ParsePart(part).ToArray();
                }
                catch (Exception ex)
                {
                    var functioName = $"{reader.GetType()}.ParseJson";
                    //Log.Error(ex, "{outputLogError}: Error on {functioName} with json {json}", "OUTPUTLOG", functioName, json);
                    Log.Error(ex, "{outputLogError}: Error on {functioName} with json", "OUTPUTLOG", functioName);
                }

            return null;
        }
    }
}