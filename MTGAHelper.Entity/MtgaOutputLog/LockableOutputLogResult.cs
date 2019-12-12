using MTGAHelper.Entity;
using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Entity.UserHistory;
using MTGAHelper.Lib.Config.Users;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger;
using MTGAHelper.Lib.UserHistory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog
{
    public interface ILockableOutputLogResultData<T>
    {
        IList<InfoByDate<T>> GetData();
        void SetData(IList<InfoByDate<T>> newData);
        void AddData(InfoByDate<T> newData);
    }

    public class LockableOutputLogResultData<T> : ILockableOutputLogResultData<T>
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
        //public Dictionary<InfoByDateKeyEnum, ILockableOutputLogResultData<object>> Data { get; private set; } = new Dictionary<InfoByDateKeyEnum, ILockableOutputLogResultData<object>>
        //{
        //    { InfoByDateKeyEnum.Rank,  new LockableOutputLogResultData<object>()  }
        //};

        public LockableOutputLogResultData<ICollection<ConfigModelRankInfo>> RankSyntheticByDate { get; set; } = new LockableOutputLogResultData<ICollection<ConfigModelRankInfo>>();
        public LockableOutputLogResultData<Inventory> InventoryByDate { get; set; } = new LockableOutputLogResultData<Inventory>();
        public LockableOutputLogResultData<Dictionary<int, int>> CollectionByDate { get; set; } = new LockableOutputLogResultData<Dictionary<int, int>>();
        public LockableOutputLogResultData<IList<MatchResult>> MatchesByDate { get; set; } = new LockableOutputLogResultData<IList<MatchResult>>();
        public LockableOutputLogResultData<IList<PlayerQuest>> PlayerQuestsByDate { get; set; } = new LockableOutputLogResultData<IList<PlayerQuest>>();
        public LockableOutputLogResultData<Dictionary<DateTime, CrackBoosterRaw>> CrackedBoostersByDate { get; set; } = new LockableOutputLogResultData<Dictionary<DateTime, CrackBoosterRaw>>();
        public LockableOutputLogResultData<HashSet<string>> MtgaDecksFoundByDate { get; set; } = new LockableOutputLogResultData<HashSet<string>>();
        //public LockableOutputLogResultData<DateSnapshotDiff> DiffByDate { get; set; } = new LockableOutputLogResultData<DateSnapshotDiff>();
        public LockableOutputLogResultData<Dictionary<string, PlayerProgress>> PlayerProgressByDate { get; set; } = new LockableOutputLogResultData<Dictionary<string, PlayerProgress>>();
        public LockableOutputLogResultData<Dictionary<DateTime, GetPlayerProgressRaw>> PlayerProgressIntradayByDate { get; set; } = new LockableOutputLogResultData<Dictionary<DateTime, GetPlayerProgressRaw>>();
        public LockableOutputLogResultData<Dictionary<DateTime, InventoryUpdatedRaw>> InventoryUpdatesByDate { get; set; } = new LockableOutputLogResultData<Dictionary<DateTime, InventoryUpdatedRaw>>();
        public LockableOutputLogResultData<Dictionary<DateTime, PostMatchUpdateRaw>> PostMatchUpdatesByDate { get; set; } = new LockableOutputLogResultData<Dictionary<DateTime, PostMatchUpdateRaw>>();
        public LockableOutputLogResultData<IList<DraftMakePickRaw>> DraftPickProgressByDate { get; set; } = new LockableOutputLogResultData<IList<DraftMakePickRaw>>();
        public LockableOutputLogResultData<Dictionary<DateTime, DraftMakePickRaw>> DraftPickProgressIntradayByDate { get; set; } = new LockableOutputLogResultData<Dictionary<DateTime, DraftMakePickRaw>>();
        public LockableOutputLogResultData<Dictionary<DateTime, CompleteVaultRaw>> VaultsOpenedByDate { get; set; } = new LockableOutputLogResultData<Dictionary<DateTime, CompleteVaultRaw>>();
        public LockableOutputLogResultData<Dictionary<DateTime, Dictionary<int, int>>> CollectionIntradayByDate { get; set; } = new LockableOutputLogResultData<Dictionary<DateTime, Dictionary<int, int>>>();
        public LockableOutputLogResultData<Dictionary<DateTime, Inventory>> InventoryIntradayByDate { get; set; } = new LockableOutputLogResultData<Dictionary<DateTime, Inventory>>();
        public LockableOutputLogResultData<Dictionary<DateTime, EventClaimPrizeRaw>> EventClaimPriceByDate { get; set; } = new LockableOutputLogResultData<Dictionary<DateTime, EventClaimPrizeRaw>>();
        public LockableOutputLogResultData<Dictionary<DateTime, MythicRatingUpdatedRaw>> MythicRatingUpdatedByDate { get; set; } = new LockableOutputLogResultData<Dictionary<DateTime, MythicRatingUpdatedRaw>>();
        public LockableOutputLogResultData<Dictionary<DateTime, PayEntryRaw>> PayEntryByDate { get; set; } = new LockableOutputLogResultData<Dictionary<DateTime, PayEntryRaw>>();
        public LockableOutputLogResultData<Dictionary<DateTime, GetCombinedRankInfoRaw>> CombinedRankInfoByDate { get; set; } = new LockableOutputLogResultData<Dictionary<DateTime, GetCombinedRankInfoRaw>>();
        public LockableOutputLogResultData<Dictionary<DateTime, RankUpdatedRaw>> RankUpdatedByDate { get; set; } = new LockableOutputLogResultData<Dictionary<DateTime, RankUpdatedRaw>>();

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

            //foreach (var dataType in Data.Keys)
            //{
            //    foreach (var data in Data[dataType].GetData())
            //        CreateOrGetDateSnapshotInfo(data.DateTime).
            //}

            foreach (var collection in CollectionByDate.GetData())
                CreateOrGetDateSnapshotInfo(collection.DateTime).Collection = collection.Info;

            foreach (var inventory in InventoryByDate.GetData())
                CreateOrGetDateSnapshotInfo(inventory.DateTime).Inventory = inventory.Info;

            foreach (var progress in PlayerProgressByDate.GetData())
                CreateOrGetDateSnapshotInfo(progress.DateTime).PlayerProgress = progress.Info;

            foreach (var progress in PlayerProgressIntradayByDate.GetData())
                CreateOrGetDateSnapshotInfo(progress.DateTime).PlayerProgressIntraday = progress.Info;

            foreach (var matches in MatchesByDate.GetData())
                CreateOrGetDateSnapshotInfo(matches.DateTime).Matches = matches.Info;

            foreach (var quests in PlayerQuestsByDate.GetData())
                CreateOrGetDateSnapshotInfo(quests.DateTime).PlayerQuests = quests.Info;

            foreach (var boosters in CrackedBoostersByDate.GetData())
                CreateOrGetDateSnapshotInfo(boosters.DateTime).CrackedBoosters = boosters.Info;

            foreach (var rankInfo in RankSyntheticByDate.GetData())
                CreateOrGetDateSnapshotInfo(rankInfo.DateTime).RankSynthetic = rankInfo.Info;

            foreach (var inventoryUpdate in InventoryUpdatesByDate.GetData())
                CreateOrGetDateSnapshotInfo(inventoryUpdate.DateTime).InventoryUpdates = inventoryUpdate.Info;

            foreach (var postMatchUpdate in PostMatchUpdatesByDate.GetData())
                CreateOrGetDateSnapshotInfo(postMatchUpdate.DateTime).PostMatchUpdates = postMatchUpdate.Info;

            foreach (var draftPicks in DraftPickProgressByDate.GetData())
                CreateOrGetDateSnapshotInfo(draftPicks.DateTime).DraftPickProgress = draftPicks.Info;

            foreach (var draftPicks in DraftPickProgressIntradayByDate.GetData())
                CreateOrGetDateSnapshotInfo(draftPicks.DateTime).DraftPickProgressIntraday = draftPicks.Info;

            foreach (var payEntry in PayEntryByDate.GetData())
                CreateOrGetDateSnapshotInfo(payEntry.DateTime).PayEntry = payEntry.Info;

            foreach (var mythicRatingUpdated in MythicRatingUpdatedByDate.GetData())
                CreateOrGetDateSnapshotInfo(mythicRatingUpdated.DateTime).MythicRatingUpdated = mythicRatingUpdated.Info;

            foreach (var combinedRankInfo in CombinedRankInfoByDate.GetData())
                CreateOrGetDateSnapshotInfo(combinedRankInfo.DateTime).CombinedRankInfo = combinedRankInfo.Info;

            foreach (var rankUpdated in RankUpdatedByDate.GetData())
                CreateOrGetDateSnapshotInfo(rankUpdated.DateTime).RankUpdated = rankUpdated.Info;

            foreach (var eventClaimPrice in EventClaimPriceByDate.GetData())
                CreateOrGetDateSnapshotInfo(eventClaimPrice.DateTime).EventClaimPrize = eventClaimPrice.Info;

            foreach (var mtgaDecksfound in MtgaDecksFoundByDate.GetData())
                CreateOrGetDateSnapshotInfo(mtgaDecksfound.DateTime).MtgaDecksFound = mtgaDecksfound.Info;

            foreach (var vaultsOpened in VaultsOpenedByDate.GetData())
                CreateOrGetDateSnapshotInfo(vaultsOpened.DateTime).VaultsOpened = vaultsOpened.Info;

            foreach (var collectionIntraday in CollectionIntradayByDate.GetData())
                CreateOrGetDateSnapshotInfo(collectionIntraday.DateTime).CollectionIntraday = collectionIntraday.Info;

            foreach (var inventoryIntraday in InventoryIntradayByDate.GetData())
                CreateOrGetDateSnapshotInfo(inventoryIntraday.DateTime).InventoryIntraday = inventoryIntraday.Info;

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

        public InfoByDate<ICollection<ConfigModelRankInfo>> GetLastRankInfo() => RankSyntheticByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
            ?? new InfoByDate<ICollection<ConfigModelRankInfo>>(default(DateTime),
                new ConfigModelRankInfo[] {
                    new ConfigModelRankInfo(RankFormatEnum.Constructed),
                    new ConfigModelRankInfo(RankFormatEnum.Limited)
                });

        //public InfoByDate<Inventory> GetLastInventory() => InventoryByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
        //    ?? new InfoByDate<Inventory>(default(DateTime), new Inventory());

        //public InfoByDate<Dictionary<DateTime, InventoryUpdatedRaw>> GetLastInventoryUpdates() => InventoryUpdatesByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
        //    ?? new InfoByDate<Dictionary<DateTime, InventoryUpdatedRaw>>(default(DateTime), new Dictionary<DateTime, InventoryUpdatedRaw>());

        //public InfoByDate<Dictionary<DateTime, PostMatchUpdateRaw>> GetLastPostMatchUpdates() => PostMatchUpdatesByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
        //    ?? new InfoByDate<Dictionary<DateTime, PostMatchUpdateRaw>>(default(DateTime), new Dictionary<DateTime, PostMatchUpdateRaw>());

        public InfoByDate<Dictionary<string, PlayerProgress>> GetLastProgress() => PlayerProgressByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
            ?? new InfoByDate<Dictionary<string, PlayerProgress>>(default(DateTime), new Dictionary<string, PlayerProgress>());

        public InfoByDate<IList<PlayerQuest>> GetLastQuests() => PlayerQuestsByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
            ?? new InfoByDate<IList<PlayerQuest>>(default(DateTime), new List<PlayerQuest>());

        //public InfoByDate<Dictionary<DateTime, CrackBoosterRaw>> GetLastCrackedBoosters() => CrackedBoostersByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
        //    ?? new InfoByDate<Dictionary<DateTime, CrackBoosterRaw>>(default(DateTime), new Dictionary<DateTime, CrackBoosterRaw>());

        public InfoByDate<IList<DraftMakePickRaw>> GetLastDraftPickProgress() => DraftPickProgressByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
            ?? new InfoByDate<IList<DraftMakePickRaw>>(default(DateTime), new List<DraftMakePickRaw>());

        //public InfoByDate<Dictionary<DateTime, DraftMakePickRaw>> GetLastDraftPickProgressIntraday() => DraftPickProgressIntradayByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
        //    ?? new InfoByDate<Dictionary<DateTime, DraftMakePickRaw>>(default(DateTime), new Dictionary<DateTime, DraftMakePickRaw>());

        public InfoByDate<HashSet<string>> GetLastMtgaDecksFound() => MtgaDecksFoundByDate.GetData().OrderBy(i => i.DateTime).LastOrDefault()
            ?? new InfoByDate<HashSet<string>>(default(DateTime), new HashSet<string>());

        public DateSnapshotInfo GetForDate(DateTime dateFor)
        {
            var resultForDate = new DateSnapshotInfo
            {
                Date = dateFor,
                Collection = CollectionByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<int, int>(),
                //Decks = DecksByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new ConfigModelRawDeck[0],
                Inventory = InventoryByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Inventory(),
                Matches = MatchesByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new MatchResult[0],
                RankSynthetic = RankSyntheticByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new ConfigModelRankInfo[0],
                PlayerProgress = PlayerProgressByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<string, PlayerProgress>(),
                PlayerProgressIntraday = PlayerProgressIntradayByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, GetPlayerProgressRaw>(),
                InventoryUpdates = InventoryUpdatesByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, InventoryUpdatedRaw>(),
                PostMatchUpdates = PostMatchUpdatesByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, PostMatchUpdateRaw>(),
                PlayerQuests = PlayerQuestsByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new PlayerQuest[0],
                CrackedBoosters = CrackedBoostersByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, CrackBoosterRaw>(),
                DraftPickProgress = DraftPickProgressByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new DraftMakePickRaw[0],
                DraftPickProgressIntraday = DraftPickProgressIntradayByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, DraftMakePickRaw>(),
                MtgaDecksFound = MtgaDecksFoundByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new HashSet<string>(),
                VaultsOpened = VaultsOpenedByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, CompleteVaultRaw>(),
                CollectionIntraday = CollectionIntradayByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, Dictionary<int, int>>(),
                InventoryIntraday = InventoryIntradayByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, Inventory>(),
                CombinedRankInfo = CombinedRankInfoByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, GetCombinedRankInfoRaw>(),
                EventClaimPrize = EventClaimPriceByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, EventClaimPrizeRaw>(),
                PayEntry = PayEntryByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, PayEntryRaw>(),
                RankUpdated = RankUpdatedByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, RankUpdatedRaw>(),
                MythicRatingUpdated = MythicRatingUpdatedByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, MythicRatingUpdatedRaw>(),
            };

            //var diff = DiffByDate.GetData().SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new DateSnapshotDiff();

            //return (resultForDate, diff);
            return resultForDate;
        }
    }
}
