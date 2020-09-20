using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MTGAHelper.Entity;
using MTGAHelper.Entity.MtgaOutputLog;
using MTGAHelper.Lib.OutputLogParser;
using MTGAHelper.Lib.OutputLogParser.Models;

namespace MTGAHelper.ConsoleSync.Services
{
    public class LogFileProcessor
    {
        private readonly ReaderMtgaOutputLog reader;
        private readonly ServerApiCaller api;
        private readonly Dictionary<int, Card> dictAllCards;

        public LogFileProcessor(
            ReaderMtgaOutputLog reader,
            ServerApiCaller api,
            Dictionary<int, Card> dictAllCards
        )
        {
            this.reader = reader;
            this.api = api;
            this.dictAllCards = dictAllCards;
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
            var totalCards = result.GetLastCollection().Info
                .Where(c => dictAllCards[c.Key].type.StartsWith("Basic Land") == false)
                .Sum(c => c.Value);
            Console.WriteLine($"Processing completed. Found {matchCount} match{matchPlural} and {totalCards} cards in collection");

            // Upload
            Console.WriteLine("Uploading data to server...");
            api.UploadOutputLogResult(userId, result, result2);

            Console.WriteLine("Success! Your data on the server was updated.");
        }
    }
}
