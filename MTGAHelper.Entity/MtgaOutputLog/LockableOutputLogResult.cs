using MTGAHelper.Entity;
using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.Config.Users;
using MTGAHelper.Lib.UserHistory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog
{
    public class LockableOutputLogResultData<T>
    {
        object lockData = new object();
        IList<InfoByDate<T>> data = new List<InfoByDate<T>>();

        public IList<InfoByDate<T>> GetData()
        {
            lock (lockData)
                return data;
        }

        public void SetData(IList<InfoByDate<T>> newData)
        {
            lock (lockData)
                data = newData;
        }

        public void AddData(InfoByDate<T> newData)
        {
            lock (lockData)
                data.Add(newData);
        }
    }

    [Serializable]
    public class LockableOutputLogResult
    {
        public LockableOutputLogResultData<ICollection<ConfigModelRankInfo>> RankInfoByDate { get; set; } = new LockableOutputLogResultData<ICollection<ConfigModelRankInfo>>();
        public LockableOutputLogResultData<Inventory> InventoryByDate { get; set; } = new LockableOutputLogResultData<Inventory>();
        public LockableOutputLogResultData<Dictionary<int, int>> CollectionByDate { get; set; } = new LockableOutputLogResultData<Dictionary<int, int>>();
        public LockableOutputLogResultData<IList<MatchResult>> MatchesByDate { get; set; } = new LockableOutputLogResultData<IList<MatchResult>>();
        public LockableOutputLogResultData<IList<PlayerQuest>> PlayerQuestsByDate { get; set; } = new LockableOutputLogResultData<IList<PlayerQuest>>();
        //public LockableOutputLogResultData<IList<MtgaDeck>> DecksGlobal { get; set; } = new LockableOutputLogResultData<IList<MtgaDeck>>();
        public LockableOutputLogResultData<DateSnapshotDiff> DiffByDate { get; set; } = new LockableOutputLogResultData<DateSnapshotDiff>();
        public LockableOutputLogResultData<Dictionary<string, PlayerProgress>> PlayerProgressByDate { get; set; } = new LockableOutputLogResultData<Dictionary<string, PlayerProgress>>();
        public LockableOutputLogResultData<Dictionary<DateTime, InventoryUpdatedRaw>> InventoryUpdatesByDate { get; set; } = new LockableOutputLogResultData<Dictionary<DateTime, InventoryUpdatedRaw>>();
        public LockableOutputLogResultData<Dictionary<DateTime, PostMatchUpdateRaw>> PostMatchUpdatesByDate { get; set; } = new LockableOutputLogResultData<Dictionary<DateTime, PostMatchUpdateRaw>>();
        public LockableOutputLogResultData<IList<DraftPickProgress>> DraftPickProgressByDate { get; set; } = new LockableOutputLogResultData<IList<DraftPickProgress>>();
        
        //InfoByDate<ICollection<CardWithAmount>> lastCollectionInMemory = null;

        public uint LastUploadHash { get; set; }

        public ICollection<DateSnapshotInfo> BuildHistory()
        {
            var result = new List<DateSnapshotInfo>();

            Func<DateTime, DateSnapshotInfo> CreateOrGetDateSnapshotInfo = (date) =>
            {
                var dateOnly = date.Date;
                var info = result.FirstOrDefault(i => i.Date == dateOnly);
                if (info == null)
                {
                    info = new DateSnapshotInfo(dateOnly);
                    result.Add(info);
                }

                return info;
            };

            foreach (var collection in CollectionByDate.GetData())
                CreateOrGetDateSnapshotInfo(collection.DateTime).Collection = collection.Info;

            foreach (var inventory in InventoryByDate.GetData())
                CreateOrGetDateSnapshotInfo(inventory.DateTime).Inventory = inventory.Info;

            foreach (var progress in PlayerProgressByDate.GetData())
                CreateOrGetDateSnapshotInfo(progress.DateTime).PlayerProgress = progress.Info;

            foreach (var matches in MatchesByDate.GetData())
                CreateOrGetDateSnapshotInfo(matches.DateTime).Matches = matches.Info;

            foreach (var quests in PlayerQuestsByDate.GetData())
                CreateOrGetDateSnapshotInfo(quests.DateTime).PlayerQuests = quests.Info;

            foreach (var rankInfo in RankInfoByDate.GetData())
                CreateOrGetDateSnapshotInfo(rankInfo.DateTime).RankInfo = rankInfo.Info;

            foreach (var inventoryUpdate in InventoryUpdatesByDate.GetData())
                CreateOrGetDateSnapshotInfo(inventoryUpdate.DateTime).InventoryUpdates = inventoryUpdate.Info;

            foreach (var postMatchUpdate in PostMatchUpdatesByDate.GetData())
                CreateOrGetDateSnapshotInfo(postMatchUpdate.DateTime).PostMatchUpdates = postMatchUpdate.Info;

            foreach (var draftPicks in DraftPickProgressByDate.GetData())
                CreateOrGetDateSnapshotInfo(draftPicks.DateTime).DraftPickProgress = draftPicks.Info;

            //foreach (var decks in DecksByDate.GetData())
            //    CreateOrGetDateSnapshotInfo(decks.DateTime).Decks = decks.Info;

            //ICollection<ConfigModelRankInfo> previousRank = null;
            foreach (var s in result)
            {
                //s.Wins = s.Matches.Count(i => i.Outcome == GameOutcomeEnum.Victory);
                //s.Losses = s.Matches.Count(i => i.Outcome == GameOutcomeEnum.Defeat);
                s.OutcomesByMode = s.Matches
                    .GroupBy(i => i.EventName)
                    .ToDictionary(i => i.Key, i => new Outcomes
                    {
                        Wins = i.Count(x => x.Outcome == GameOutcomeEnum.Victory),
                        Losses = i.Count(x => x.Outcome == GameOutcomeEnum.Defeat),
                    });

                //s.ConstructedRank = (s.RankInfo.FirstOrDefault(i => i.Format == ConfigModelRankInfoFormatEnum.Constructed) ??
                //                    previousRank?.FirstOrDefault(i => i.Format == ConfigModelRankInfoFormatEnum.Constructed))?.ToString() ?? "N/A";
                //s.LimitedRank = (s.RankInfo.FirstOrDefault(i => i.Format == ConfigModelRankInfoFormatEnum.Limited) ??
                //                    previousRank?.FirstOrDefault(i => i.Format == ConfigModelRankInfoFormatEnum.Limited))?.ToString() ?? "N/A";

                //previousRank = s.RankInfo;
            }

            return result;
        }

        public InfoByDate<Dictionary<int, int>> GetLastCollection() => CollectionByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
            ?? new InfoByDate<Dictionary<int, int>>(default(DateTime), new Dictionary<int, int>());

        public InfoByDate<ICollection<CardWithAmount>> GetLastCollectionInMemory(RawDeckConverter converter)
        {
            InfoByDate<ICollection<CardWithAmount>> lastCollectionInMemory = null;
            var info = GetLastCollection();
            //if (lastCollectionInMemory == null || lastCollectionInMemory.DateTime == default(DateTime))
            {
                //var info = GetLastCollection();
                lastCollectionInMemory = new InfoByDate<ICollection<CardWithAmount>>(info.DateTime, converter.LoadCollection(info.Info));
            }

            return lastCollectionInMemory;
        }

        public InfoByDate<ICollection<ConfigModelRankInfo>> GetLastRankInfo() => RankInfoByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
            ?? new InfoByDate<ICollection<ConfigModelRankInfo>>(default(DateTime),
                new ConfigModelRankInfo[] {
                    new ConfigModelRankInfo(ConfigModelRankInfoFormatEnum.Constructed),
                    new ConfigModelRankInfo(ConfigModelRankInfoFormatEnum.Limited)
                });

        public InfoByDate<Inventory> GetLastInventory() => InventoryByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
            ?? new InfoByDate<Inventory>(default(DateTime), new Inventory());

        public InfoByDate<Dictionary<DateTime, InventoryUpdatedRaw>> GetLastInventoryUpdates() => InventoryUpdatesByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
            ?? new InfoByDate<Dictionary<DateTime, InventoryUpdatedRaw>>(default(DateTime), new Dictionary<DateTime, InventoryUpdatedRaw>());

        public InfoByDate<Dictionary<DateTime, PostMatchUpdateRaw>> GetLastPostMatchUpdates() => PostMatchUpdatesByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
            ?? new InfoByDate<Dictionary<DateTime, PostMatchUpdateRaw>>(default(DateTime), new Dictionary<DateTime, PostMatchUpdateRaw>());

        public InfoByDate<Dictionary<string, PlayerProgress>> GetLastProgress() => PlayerProgressByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
            ?? new InfoByDate<Dictionary<string, PlayerProgress>>(default(DateTime), new Dictionary<string, PlayerProgress>());

        public InfoByDate<IList<PlayerQuest>> GetLastQuests() => PlayerQuestsByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
            ?? new InfoByDate<IList<PlayerQuest>>(default(DateTime), new List<PlayerQuest>());

        public InfoByDate<IList<DraftPickProgress>> GetLastDraftPickProgress() => DraftPickProgressByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
            ?? new InfoByDate<IList<DraftPickProgress>>(default(DateTime), new List<DraftPickProgress>());

        public (DateSnapshotInfo info, DateSnapshotDiff diff) GetForDate(DateTime dateFor)
        {
            var resultForDate = new DateSnapshotInfo
            {
                Date = dateFor,
                Collection = CollectionByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<int, int>(),
                //Decks = DecksByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new ConfigModelRawDeck[0],
                Inventory = InventoryByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Inventory(),
                Matches = MatchesByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new MatchResult[0],
                RankInfo = RankInfoByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new ConfigModelRankInfo[0],
                PlayerProgress = PlayerProgressByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<string, PlayerProgress>(),
                InventoryUpdates = InventoryUpdatesByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, InventoryUpdatedRaw>(),
                PostMatchUpdates = PostMatchUpdatesByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, PostMatchUpdateRaw>(),
                PlayerQuests = PlayerQuestsByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new PlayerQuest[0],
                DraftPickProgress = DraftPickProgressByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new DraftPickProgress[0],
            };

            var diff = DiffByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new DateSnapshotDiff();

            return (resultForDate, diff);
        }
    }
}
