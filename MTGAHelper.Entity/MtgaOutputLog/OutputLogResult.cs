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
    [Serializable]
    public class OutputLogResult
    {
        //InfoByDate<ICollection<CardWithAmount>> lastCollectionInMemory = null;
        public List<ConfigModelRawDeck> DecksSynthetic { get; set; } = new List<ConfigModelRawDeck>();

        public IList<InfoByDate<List<ConfigModelRankInfo>>> RankSyntheticByDate { get; set; } = new List<InfoByDate<List<ConfigModelRankInfo>>>();
        public IList<InfoByDate<HashSet<string>>> MtgaDecksFoundByDate { get; set; } = new List<InfoByDate<HashSet<string>>>();
        public IList<InfoByDate<Inventory>> InventoryByDate { get; set; } = new List<InfoByDate<Inventory>>();
        public IList<InfoByDate<Dictionary<int, int>>> CollectionByDate { get; set; } = new List<InfoByDate<Dictionary<int, int>>>();
        public IList<InfoByDate<List<MatchResult>>> MatchesByDate { get; set; } = new List<InfoByDate<List<MatchResult>>>();
        public IList<InfoByDate<List<PlayerQuest>>> PlayerQuestsByDate { get; set; } = new List<InfoByDate<List<PlayerQuest>>>();
        public IList<InfoByDate<Dictionary<string, PlayerProgress>>> PlayerProgressByDate { get; set; } = new List<InfoByDate<Dictionary<string, PlayerProgress>>>();
        public IList<InfoByDate<List<DraftMakePickRaw>>> DraftPickProgressByDate { get; set; } = new List<InfoByDate<List<DraftMakePickRaw>>>();
        //public IList<InfoByDate<DateSnapshotDiff>> DiffByDate { get; set; } = new List<InfoByDate<DateSnapshotDiff>>();
        public IList<InfoByDate<Dictionary<DateTime, CrackBoosterRaw>>> CrackedBoostersByDate { get; set; } = new List<InfoByDate<Dictionary<DateTime, CrackBoosterRaw>>>();
        public IList<InfoByDate<Dictionary<DateTime, GetPlayerProgressRaw>>> PlayerProgressIntradayByDate { get; set; } = new List<InfoByDate<Dictionary<DateTime, GetPlayerProgressRaw>>>();
        public IList<InfoByDate<Dictionary<DateTime, InventoryUpdatedRaw>>> InventoryUpdatesByDate { get; set; } = new List<InfoByDate<Dictionary<DateTime, InventoryUpdatedRaw>>>();
        public IList<InfoByDate<Dictionary<DateTime, PostMatchUpdateRaw>>> PostMatchUpdatesByDate { get; set; } = new List<InfoByDate<Dictionary<DateTime, PostMatchUpdateRaw>>>();
        public IList<InfoByDate<Dictionary<DateTime, CompleteVaultRaw>>> VaultsOpenedByDate { get; set; } = new List<InfoByDate<Dictionary<DateTime, CompleteVaultRaw>>>();
        public IList<InfoByDate<Dictionary<DateTime, DraftMakePickRaw>>> DraftPickProgressIntradayByDate { get; set; } = new List<InfoByDate<Dictionary<DateTime, DraftMakePickRaw>>>();
        public IList<InfoByDate<Dictionary<DateTime, RankUpdatedRaw>>> RankUpdatedByDate { get; set; } = new List<InfoByDate<Dictionary<DateTime, RankUpdatedRaw>>>();
        public IList<InfoByDate<Dictionary<DateTime, EventClaimPrizeRaw>>> EventClaimPriceByDate { get; set; } = new List<InfoByDate<Dictionary<DateTime, EventClaimPrizeRaw>>>();

        public IList<InfoByDate<Dictionary<DateTime, MythicRatingUpdatedRaw>>> MythicRatingUpdatedByDate { get; set; } = new List<InfoByDate<Dictionary<DateTime, MythicRatingUpdatedRaw>>>();
        public IList<InfoByDate<Dictionary<DateTime, PayEntryRaw>>> PayEntryByDate { get; set; } = new List<InfoByDate<Dictionary<DateTime, PayEntryRaw>>>();
        public IList<InfoByDate<Dictionary<DateTime, GetCombinedRankInfoRaw>>> CombinedRankInfoByDate { get; set; } = new List<InfoByDate<Dictionary<DateTime, GetCombinedRankInfoRaw>>>();
        public IList<InfoByDate<Dictionary<DateTime, Inventory>>> InventoryIntradayByDate { get; set; } = new List<InfoByDate<Dictionary<DateTime, Inventory>>>();
        public IList<InfoByDate<Dictionary<DateTime, Dictionary<int, int>>>> CollectionIntradayByDate { get; set; } = new List<InfoByDate<Dictionary<DateTime, Dictionary<int, int>>>>();

        public GetSeasonAndRankDetailRaw SeasonAndRankDetail { get; set; } = new GetSeasonAndRankDetailRaw { currentSeason = new CurrentSeason() };

        public string PlayerName { get; set; }
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

            foreach (var mtgaDeck in MtgaDecksFoundByDate)
                CreateOrGetDateSnapshotInfo(mtgaDeck.DateTime).MtgaDecksFound = mtgaDeck.Info;

            foreach (var collection in CollectionByDate)
                CreateOrGetDateSnapshotInfo(collection.DateTime).Collection = collection.Info;

            foreach (var inventory in InventoryByDate)
                CreateOrGetDateSnapshotInfo(inventory.DateTime).Inventory = inventory.Info;

            foreach (var progress in PlayerProgressByDate)
                CreateOrGetDateSnapshotInfo(progress.DateTime).PlayerProgress = progress.Info;

            foreach (var progress in PlayerProgressIntradayByDate)
                CreateOrGetDateSnapshotInfo(progress.DateTime).PlayerProgressIntraday = progress.Info;

            foreach (var quest in PlayerQuestsByDate)
                CreateOrGetDateSnapshotInfo(quest.DateTime).PlayerQuests = quest.Info;

            foreach (var matches in MatchesByDate)
                CreateOrGetDateSnapshotInfo(matches.DateTime).Matches = matches.Info;

            foreach (var rankInfo in RankSyntheticByDate)
                CreateOrGetDateSnapshotInfo(rankInfo.DateTime).RankSynthetic = rankInfo.Info;

            foreach (var inventoryUpdate in InventoryUpdatesByDate)
                CreateOrGetDateSnapshotInfo(inventoryUpdate.DateTime).InventoryUpdates = inventoryUpdate.Info;

            foreach (var postMatchUpdate in PostMatchUpdatesByDate)
                CreateOrGetDateSnapshotInfo(postMatchUpdate.DateTime).PostMatchUpdates = postMatchUpdate.Info;

            foreach (var draftPick in DraftPickProgressByDate)
                CreateOrGetDateSnapshotInfo(draftPick.DateTime).DraftPickProgress = draftPick.Info;

            foreach (var draftPick in DraftPickProgressIntradayByDate)
                CreateOrGetDateSnapshotInfo(draftPick.DateTime).DraftPickProgressIntraday = draftPick.Info;

            foreach (var rankUpdated in RankUpdatedByDate)
                CreateOrGetDateSnapshotInfo(rankUpdated.DateTime).RankUpdated = rankUpdated.Info;

            foreach (var eventClaimPrice in EventClaimPriceByDate)
                CreateOrGetDateSnapshotInfo(eventClaimPrice.DateTime).EventClaimPrize = eventClaimPrice.Info;

            foreach (var mythicRatingUpdated in MythicRatingUpdatedByDate)
                CreateOrGetDateSnapshotInfo(mythicRatingUpdated.DateTime).MythicRatingUpdated = mythicRatingUpdated.Info;

            foreach (var payEntry in PayEntryByDate)
                CreateOrGetDateSnapshotInfo(payEntry.DateTime).PayEntry = payEntry.Info;

            foreach (var combinedRankInfo in CombinedRankInfoByDate)
                CreateOrGetDateSnapshotInfo(combinedRankInfo.DateTime).CombinedRankInfo = combinedRankInfo.Info;

            foreach (var booster in CrackedBoostersByDate)
                CreateOrGetDateSnapshotInfo(booster.DateTime).CrackedBoosters = booster.Info;

            foreach (var vault in VaultsOpenedByDate)
                CreateOrGetDateSnapshotInfo(vault.DateTime).VaultsOpened = vault.Info;

            foreach (var vault in CollectionIntradayByDate)
                CreateOrGetDateSnapshotInfo(vault.DateTime).CollectionIntraday = vault.Info;

            foreach (var vault in InventoryIntradayByDate)
                CreateOrGetDateSnapshotInfo(vault.DateTime).InventoryIntraday = vault.Info;

            //foreach (var decks in DecksByDate)
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

        public List<string> GetDates()
        {
            var dates = InventoryIntradayByDate.Select(i => i.DateTime.Date)
                .Union(PostMatchUpdatesByDate.Select(i => i.DateTime.Date))
                .Union(MatchesByDate.Select(i => i.DateTime.Date))
                .Union(CollectionIntradayByDate.Select(i => i.DateTime.Date))
                .Distinct();

            return dates.Select(i => i.ToString("yyyyMMdd")).ToList();
        }

        public InfoByDate<Dictionary<int, int>> GetLastCollection() => CollectionByDate.OrderBy(i => i.DateTime).LastOrDefault()
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

        //public InfoByDate<ICollection<ConfigModelRankInfo>> GetLastRank() => RankSyntheticByDate.OrderBy(i => i.DateTime).LastOrDefault()
        //    ?? new InfoByDate<ICollection<ConfigModelRankInfo>>(default(DateTime),
        //        new ConfigModelRankInfo[] {
        //            new ConfigModelRankInfo(RankFormatEnum.Constructed),
        //            new ConfigModelRankInfo(RankFormatEnum.Limited)
        //        });

        //public InfoByDate<Inventory> GetLastInventory() => InventoryByDate.OrderBy(i => i.DateTime).LastOrDefault()
        //    ?? new InfoByDate<Inventory>(default(DateTime), new Inventory());

        //public InfoByDate<IList<PlayerQuest>> GetLastPlayerQuests() => PlayerQuestsByDate.OrderBy(i => i.DateTime).LastOrDefault()
        //    ?? new InfoByDate<IList<PlayerQuest>>(default(DateTime), new PlayerQuest[0]);

        //public InfoByDate<Dictionary<DateTime, CrackBoosterRaw>> GetLastCrackedBoosters() => CrackedBoostersByDate.OrderBy(i => i.DateTime).LastOrDefault()
        //    ?? new InfoByDate<Dictionary<DateTime, CrackBoosterRaw>>(default(DateTime), new Dictionary<DateTime, CrackBoosterRaw>());

        //public InfoByDate<IList<DraftMakePickRaw>> GetLastDraftPickProgress() => DraftPickProgressByDate.OrderBy(i => i.DateTime).LastOrDefault()
        //    ?? new InfoByDate<IList<DraftMakePickRaw>>(default(DateTime), new DraftMakePickRaw[0]);

        //public InfoByDate<Dictionary<DateTime, DraftMakePickRaw>> GetLastDraftPickProgressIntraday() => DraftPickProgressIntradayByDate.OrderBy(i => i.DateTime).LastOrDefault()
        //    ?? new InfoByDate<Dictionary<DateTime, DraftMakePickRaw>>(default(DateTime), new Dictionary<DateTime, DraftMakePickRaw>());

        public DateSnapshotInfo GetForDate(DateTime dateFor)
        {
            var resultForDate = new DateSnapshotInfo
            {
                Date = dateFor,
                MtgaDecksFound = MtgaDecksFoundByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new HashSet<string>(),
                Collection = CollectionByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<int, int>(),
                //Decks = DecksByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new ConfigModelRawDeck[0],
                Inventory = InventoryByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Inventory(),
                Matches = MatchesByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new List<MatchResult>(),
                PlayerQuests = PlayerQuestsByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new List<PlayerQuest>(),
                CrackedBoosters = CrackedBoostersByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, CrackBoosterRaw>(),
                RankSynthetic = RankSyntheticByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new List<ConfigModelRankInfo>(),
                PlayerProgress = PlayerProgressByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<string, PlayerProgress>(),
                PlayerProgressIntraday = PlayerProgressIntradayByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, GetPlayerProgressRaw>(),
                InventoryUpdates = InventoryUpdatesByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, InventoryUpdatedRaw>(),
                PostMatchUpdates = PostMatchUpdatesByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, PostMatchUpdateRaw>(),
                DraftPickProgress = DraftPickProgressByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new List<DraftMakePickRaw>(),
                DraftPickProgressIntraday = DraftPickProgressIntradayByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, DraftMakePickRaw>(),
                VaultsOpened = VaultsOpenedByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, CompleteVaultRaw>(),
                CollectionIntraday = CollectionIntradayByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, Dictionary<int, int>>(),
                InventoryIntraday = InventoryIntradayByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, Inventory>(),
                RankUpdated = RankUpdatedByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, RankUpdatedRaw>(),
                CombinedRankInfo = CombinedRankInfoByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, GetCombinedRankInfoRaw>(),
                EventClaimPrize = EventClaimPriceByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, EventClaimPrizeRaw>(),
                MythicRatingUpdated = MythicRatingUpdatedByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, MythicRatingUpdatedRaw>(),
                PayEntry = PayEntryByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<DateTime, PayEntryRaw>(),
            };

            //var diff = DiffByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new DateSnapshotDiff();

            //return (resultForDate, diff);
            return resultForDate;
        }
    }
}
