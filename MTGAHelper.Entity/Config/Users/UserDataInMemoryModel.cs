using MTGAHelper.Entity;
using MTGAHelper.Entity.UserHistory;
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
        //object lockMtgaDeckHistory = new object();
        object lockHistorySummary = new object();

        public DateTime LastCompareUtc { get; set; }

        public Dictionary<string, IDeck> DecksUserDefined { get; set; } = new Dictionary<string, IDeck>();

        public CardsMissingResult CompareResult { get; set; } = new CardsMissingResult();

        //public LockableOutputLogResult HistoryDetails { get; set; } = new LockableOutputLogResult();

        IList<HistorySummaryForDate> HistorySummary { get; set; } = new List<HistorySummaryForDate>();

        //IList<ConfigModelRawDeck> MtgaDecks { get; set; } = new List<ConfigModelRawDeck>();

        public IList<HistorySummaryForDate> GetHistorySummary()
        {
            lock (lockHistorySummary)
                return HistorySummary;
        }
        public void SetHistorySummary(IList<HistorySummaryForDate> list)
        {
            lock (lockHistorySummary)
                HistorySummary = list;
        }
        //public void AddHistorySummary(HistorySummaryForDate summary)
        //{
        //    lock (lockHistorySummary)
        //        HistorySummary.Add(summary);
        //}
        //public IList<ConfigModelRawDeck> GetMtgaDecks()
        //{
        //    lock (lockMtgaDeckHistory)
        //        return MtgaDecks;
        //}
        //public void SetMtgaDecks(IList<ConfigModelRawDeck> list)
        //{
        //    lock (lockMtgaDeckHistory)
        //        MtgaDecks = list;
        //}
    }
}
