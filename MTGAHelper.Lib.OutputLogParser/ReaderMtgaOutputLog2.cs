using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MTGAHelper.Entity;
using MTGAHelper.Lib.Exceptions;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.GRE.MatchToClient;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger;
using MTGAHelper.Lib.OutputLogParser;
using MTGAHelper.Lib.OutputLogProgress;
using MTGAHelper.Utility;
using Serilog;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog
{
    public class ReaderMtgaOutputLog : ReaderMtgaOutputLogBase
    {
        // Not a normal prefix found between []
        const string INITIALIZING = "Initialize engine version:";

        const string BI_ERROR = "BIError";
        const string PREFIX_URLAPI = "Url: https://api.platform.wizards.com/";
        const string PREFIX_DETAILED_LOGS = "DETAILED LOGS:";

        const string PREFIX_CLEANUP = "CLEANUP";
        const string PREFIX_CROSSTHREADLOGGER = "UnityCrossThreadLogger";
        const string PREFIX_ACCOUNTS_STARTUP = "Accounts - Startup";
        const string PREFIX_ACCOUNTS_CLIENT = "Accounts - Client";
        const string PREFIX_STORE_AUTH_GETSKUS = "Store - Auth - Get SKUs";
        const string PREFIX_STORE_GETSKUS = "Store - Get SKUs";
        const string PREFIX_SOCIAL_CONTROLLER = "Social Controller";
        const string PREFIX_DISCORD = "Discord Rich Presence";
        const string PREFIX_CLIENT_GRE = "Client GRE";
        const string PREFIX_ACCOUNTS_SHAREDCONTEXTVIEW = "Accounts - SharedContextView";
        const string PREFIX_ACCOUNTS_LOGIN = "Accounts - Login";
        const string PREFIX_MESSAGESUMMARIZED = "Message summarized";   // because one or more GameStateMessages exceeded the 50 GameObject or 50 Annotation limit.
        const string PREFIX_EOSCLIENT = "EOSClient";
        const string PREFIX_WIZARDS_PLATFORM_SDK = "Wizards.Platform.Sdk";

        readonly IReadOnlyCollection<string> dateFormats;
        readonly IEnumerable<string> noBracePrefixes = new[] { BI_ERROR, PREFIX_URLAPI, PREFIX_DETAILED_LOGS, };

        readonly LogSplitter logSplitter;
        readonly Util util;

        bool dateFormatNullReported;

        readonly Dictionary<string, IReaderMtgaOutputLogPart> converters = new Dictionary<string, IReaderMtgaOutputLogPart>();
        readonly MtgaOutputLogResultsPreparer2 preparer2;
        readonly OutputLogMessagesBatcher outputLogMessagesBatcher;

        //Regex regexDateClean = new Regex(@"[^0-9: -\/.amp]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        readonly Regex regexDateClean2 = new Regex("[ ]{2,}", RegexOptions.Compiled);

        // 11/21/2019 10:55:39 PM LoadWrapperScene
        // 11/21/2019 10:55:42 PM Coroutine_StartupSequence
        readonly Regex regexDeduceTimestamp = new Regex("^(.*?) (?:LoadWrapperScene|Coroutine_StartupSequence)", RegexOptions.Compiled);

        string dateFormatToUse = null;

        public DateTime LogDateTime { get; private set; }

        public ReaderMtgaOutputLog(
            LogSplitter logSplitter,
            Util util,
            IPossibleDateFormats dateFormatsProvider,
            ReaderMtgaOutputLogUnityCrossThreadLogger readerCrossThreadLogger,
            ReaderMtgaOutputLogGre readerGRE,
            ReaderDetailedLogs readerDetailedLogs,
            ReaderMessageSummarized converterMessageSummarized,
            ReaderAccountsClient readerAccountsClient,
            MtgaOutputLogResultsPreparer2 preparer2,
            OutputLogMessagesBatcher outputLogMessagesBatcher)
        {
            this.logSplitter = logSplitter;
            this.util = util;
            dateFormats = dateFormatsProvider.Formats;

            //this.preparer = preparer;
            this.preparer2 = preparer2;

            this.outputLogMessagesBatcher = outputLogMessagesBatcher;

            // Handled
            converters.Add(PREFIX_CROSSTHREADLOGGER, readerCrossThreadLogger);
            converters.Add(PREFIX_CLIENT_GRE, readerGRE);
            converters.Add(PREFIX_DETAILED_LOGS, readerDetailedLogs);

            // Ignored
            converters.Add(PREFIX_CLEANUP, new ReaderIgnored());
            converters.Add(PREFIX_ACCOUNTS_STARTUP, new ReaderIgnored());
            converters.Add(PREFIX_ACCOUNTS_CLIENT, readerAccountsClient);
            converters.Add(PREFIX_STORE_AUTH_GETSKUS, new ReaderIgnored());
            converters.Add(PREFIX_STORE_GETSKUS, new ReaderIgnored());
            converters.Add(PREFIX_SOCIAL_CONTROLLER, new ReaderIgnored());
            converters.Add(PREFIX_DISCORD, new ReaderIgnored());
            converters.Add(PREFIX_ACCOUNTS_SHAREDCONTEXTVIEW, new ReaderIgnored());
            converters.Add(PREFIX_ACCOUNTS_LOGIN, new ReaderIgnored());
            converters.Add(PREFIX_MESSAGESUMMARIZED, converterMessageSummarized);
            converters.Add(PREFIX_URLAPI, new ReaderIgnored());
            converters.Add(PREFIX_EOSCLIENT, new ReaderIgnored());
            converters.Add(PREFIX_WIZARDS_PLATFORM_SDK, new ReaderIgnored());
            converters.Add("Accounts - AccountClient", new ReaderIgnored());
            converters.Add("Social", new ReaderIgnored());

            // Everything else ends up as Unknown
        }

        public void ResetPreparer() { preparer2.Reset(); }

        public async Task<ICollection<IMtgaOutputLogPartResult>> ProcessIntoMessagesAsync(string userId, Stream streamMtgaOutputLog)
        {
            var ret = new List<IMtgaOutputLogPartResult>();

            void AddMessage(string part, string prefix, DateTime logDateTime)
            {
                //if (part.Contains("==> Event.MatchCreated")) System.Diagnostics.Debugger.Break();

                if (part.Contains("Failing to send Text Message due to disconnection") || part.StartsWith(BI_ERROR))
                    return;

                try
                {
                    if (converters.ContainsKey(prefix))
                    {
                        IEnumerable<IMtgaOutputLogPartResult> messages = converters[prefix].ParsePart(part);

                        if (messages == null)
                            return;

                        if (logDateTime == default)
                            messages = messages.Where(m => m.GetType().IsDefined(typeof(AddMessageEvenIfDateNullAttribute), false));

                        ret.AddRange(messages.Select(m => m.SetCommonFields(part, prefix, logDateTime)));
                    }
                    else
                    {
                        var result = part.StartsWith(INITIALIZING) ? (IMtgaOutputLogPartResult)new IgnoredResult() : new UnknownResult();
                        ret.Add(result.SetCommonFields(part, prefix, logDateTime));
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Invalid JSON"))
                    {
                        Log.Error(ex, "(Managed) {outputLogError} Unknown error [{exceptionMessage}] in AddMessage with part: <<{part}>>", "OUTPUTLOG", ex.Message, part);
                    }
                    else
                    {
                        Log.Warning("(Managed) {outputLogError} Unknown error [{exceptionMessage}] in AddMessage", "OUTPUTLOG", ex.Message);
                    }
                    ret.Add(new UnknownResult().SetCommonFields(part, prefix, logDateTime));
                }
            }

            using (var streamReader = new StreamReader(streamMtgaOutputLog, Encoding.UTF8, true, 4096))
            {
                var builder = new StringBuilder();

                string line;
                string prefix = "";
                //DateTime lastValidLogDateTime = default(DateTime);
                //var warningDate = false;
                var bFirstLine = true;

                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    if (bFirstLine)
                    {
                        if (line.StartsWith("<!DOCTYPE html>") || line.StartsWith("<html"))
                            throw new ParseCollectionInvalidHtmlFoundException();
                        else if (line.StartsWith("using manifest Manifest_") ||
                                 line.StartsWith("SAMPLES") ||
                                 line.StartsWith("Source Manifest Hashes"))
                            throw new ParseCollectionInvalidFileException();

                        bFirstLine = false;
                    }

                    //if (line.Contains("summarized")) System.Diagnostics.Debugger.Break();

                    //if (line.Contains("[UnityCrossThreadLogger]8/3/2019 12:11:45 AM: Match to 933E5CE627155485: AuthenticateResponse")) System.Diagnostics.Debugger.Break();
                    //if (line.Contains("STATE CHANGED MatchCompleted -> Disconnected")) System.Diagnostics.Debugger.Break();

                    TryDeduceAndSetDateTime(userId, line);

                    if (!IsNewPart(line, out var newPrefix, out var dateStr))
                    {
                        builder.AppendLine(line);
                        continue;
                    }

                    // New part started
                    var part = builder.ToString().Trim();
                    builder.Clear().AppendLine(line);

                    if (!string.IsNullOrEmpty(part))
                    {
                        prefix = part.Contains(PREFIX_MESSAGESUMMARIZED)
                            ? PREFIX_MESSAGESUMMARIZED
                            : prefix;
                        AddMessage(part, prefix, LogDateTime);
                    }

                    prefix = newPrefix;
                    TrySetLogDate(userId, dateStr);
                }

                // Don't forget to process last part
                var lastPart = builder.ToString().Trim();
                AddMessage(lastPart, prefix, LogDateTime);
            }

            return ret;
        }

        bool IsNewPart(string line, out string prefix, out string dateStr)
        {
            if (line.Contains("summarized"))
            {
                prefix = dateStr = null;
                return false;
            }

            var match = util.regexPrefix.Match(line);
            if (match.Success)
            {
                prefix = match.Groups[1].Value;
                dateStr = match.Groups[2].Value;
                return true;
            }

            dateStr = null;
            foreach (var noBracePrefix in noBracePrefixes)
            {
                if (line.StartsWith(noBracePrefix))
                {
                    prefix = noBracePrefix;
                    return true;
                }
            }

            prefix = null;
            return false;
        }

        void TryDeduceAndSetDateTime(string userId, string line)
        {
            var m = regexDeduceTimestamp.Match(line);
            if (m.Success)
                TrySetLogDate(userId, m.Groups[1].Value);
        }

        void TrySetLogDate(string userId, string strDateTime)
        {
            if (string.IsNullOrWhiteSpace(strDateTime))
                return;

            //if (strDateTime.StartsWith("9/10")) System.Diagnostics.Debugger.Break();

            var dateCleaned = strDateTime
                .Replace("a. m.", "AM")
                .Replace("a.m.", "AM")
                .Replace("μ.μ.", "AM")
                .Replace("p. m.", "PM")
                .Replace("p.m.", "PM")
                .Replace("π.μ.", "PM");
            //dateCleaned = regexDateClean.Replace(dateCleaned, " ");
            dateCleaned = regexDateClean2.Replace(dateCleaned, " ");

            bool TryParseDate(string format, out DateTime date)
            {
                return DateTime.TryParseExact(dateCleaned, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            }

            // Take the closest valid date to now as the format
            //var dateRef = new DateTime(2019, 9, 10, 1, 4, 0);
            var dateRef = DateTime.UtcNow;
            var dateMaxPossibleInLog = dateRef.AddHours(14);

            if (dateFormatToUse != null)
            {
                // Weirdly, sometimes the date format is not uniform in the file!
                // so we have to reparse and validate every time
                if (TryParseDate(dateFormatToUse, out var date)
                    && date <= dateMaxPossibleInLog)
                {
                    LogDateTime = date;
                    return;
                }

                dateFormatToUse = null;
            }

            var validFormats = dateFormats
                .Select(f =>
                {
                    if (!TryParseDate(f, out var parsedDate))
                        return default;
                    var treshold = (parsedDate - dateMaxPossibleInLog).TotalSeconds;
                    return (treshold, parsedDate, f);
                })
                .Where(x => x != default && x.treshold <= 0)
                .ToArray();

            if (validFormats.Any())
            {
                (_, LogDateTime, dateFormatToUse) = validFormats.MaxBy(x => x.treshold);
                Log.Information("{outputLogError}: {userId} - Date format <{dateFormat}> selected for date <{date}>",
                    "OUTPUTLOG", userId, dateFormatToUse, strDateTime);
                return;
            }


            if (dateFormatNullReported)
                return;

            Log.Warning("{outputLogError}: {userId} - Date format NOT FOUND for date <{date}>",
                "OUTPUTLOG", userId, strDateTime);
            dateFormatNullReported = true;
        }

        public async Task<(OutputLogResult result, Guid? errorId)> LoadFileContent(string userId, Stream streamMtgaOutputLog)
        {
            var msgs = (await ProcessIntoMessagesAsync(userId, streamMtgaOutputLog))
                .Where(i => i is IgnoredResult == false && i is UnknownResult == false);

            var msgsInBatches = outputLogMessagesBatcher.BatchMessages(msgs);

            //int iMsg = 0;
            //foreach (var msg in msgs)
            //{
            //    //if (msg.Part.Contains("[UnityCrossThreadLogger]8/29/2019 5:24:17 PM: Match to 933E5CE627155485: GreToClientEvent")) System.Diagnostics.Debugger.Break();
            //    //if (msg.Part.Contains("[UnityCrossThreadLogger]8/15/2019 8:43:51 PM")) System.Diagnostics.Debugger.Break();

            //    if (msg.Timestamp == 637029592868501912) System.Diagnostics.Debugger.Break();

            //    preparer2.AddResult(msg);
            //    logSplitter.StoreLastPartWithDate(msg.Part);

            //    iMsg++;
            //}
            foreach (var batch in msgsInBatches)
            {
                foreach (var msg in batch.Value)
                {
                    preparer2.AddResult(msg);
                    logSplitter.StoreLastPartWithDate(msg.Part);
                }
            }

            var lastUploadHash = util.To32BitFnv1aHash(logSplitter.LastPartWithDate);
            preparer2.FinalizeResults(lastUploadHash);

            return (preparer2.Results, preparer2.ProducedErrorId);
        }
    }
}
