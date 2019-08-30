using MTGAHelper.Entity;
using MTGAHelper.Lib.CollectionDecksCompare;
using MTGAHelper.Lib.Config;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using MTGAHelper.Lib.UserHistory;
using System;
using System.Collections.Generic;

namespace MTGAHelper.Lib
{
    public enum DashboardStatusEnum
    {
        Unknown,
        Success,
        UserNotFound,
        CollectionNotFound,
    }

    public class UserDataInMemoryModel
    {
        public DateTime LastCompareUtc { get; set; }

        public Dictionary<string, IDeck> DecksUserDefined { get; set; } = new Dictionary<string, IDeck>();

        public CardsMissingResult CompareResult { get; set; } = new CardsMissingResult();

        public LockableOutputLogResult HistoryDetails { get; set; } = new LockableOutputLogResult();

        public IList<HistorySummaryForDate> HistorySummary { get; set; } = new List<HistorySummaryForDate>();
    }
}
