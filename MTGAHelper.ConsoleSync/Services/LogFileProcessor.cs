using System;
using System.IO;
using System.Linq;
using MTGAHelper.Entity.MtgaOutputLog;
using MTGAHelper.Lib.OutputLogParser;
using MTGAHelper.Lib.OutputLogParser.Models;

namespace MTGAHelper.ConsoleSync.Services
{
    public class LogFileProcessor
    {
        private readonly ReaderMtgaOutputLog reader;
        private readonly ServerApiCaller api;

        public LogFileProcessor(
            ReaderMtgaOutputLog reader,
            ServerApiCaller api
        )
        {
            this.reader = reader;
            this.api = api;
        }

        public void Process(string userId, string logFilePath)
        {
            OutputLogResult result;
            OutputLogResult2 result2;
            Guid? errorId;

            // Process
            Console.WriteLine("Processing file...");
            using (var stream = File.Open(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                (result, errorId, result2) = reader.LoadFileContent(userId, stream).Result;
            }

            var matchCount = result.MatchesByDate.Sum(i => i.Info.Count);
            var matchPlural = matchCount == 1 ? "" : "es";
            Console.WriteLine($"Processing completed. Found {matchCount} match{matchPlural} and {result.CollectionByDate.Sum(i => i.Info.Sum(c => c.Value))} cards in collection");

            // Upload
            Console.WriteLine("Uploading data to server...");
            api.UploadOutputLogResult(userId, result, result2);

            Console.WriteLine("Success! Your data on the server was updated.");
        }
    }
}
