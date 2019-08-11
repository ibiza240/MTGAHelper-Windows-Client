using MTGAHelper.Entity;
using MTGAHelper.Lib.Config.Users;
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

        public IList<InfoByDate<ICollection<ConfigModelRankInfo>>> RankInfoByDate { get; set; } = new List<InfoByDate<ICollection<ConfigModelRankInfo>>>();
        public IList<InfoByDate<Inventory>> InventoryByDate { get; set; } = new List<InfoByDate<Inventory>>();
        public IList<InfoByDate<Dictionary<int, int>>> CollectionByDate { get; set; } = new List<InfoByDate<Dictionary<int, int>>>();
        public IList<InfoByDate<IList<MatchResult>>> MatchesByDate { get; set; } = new List<InfoByDate<IList<MatchResult>>>();
        public IList<InfoByDate<IList<ConfigModelRawDeck>>> DecksByDate { get; set; } = new List<InfoByDate<IList<ConfigModelRawDeck>>>();
        public IList<InfoByDate<DateSnapshotDiff>> DiffByDate { get; set; } = new List<InfoByDate<DateSnapshotDiff>>();

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

            foreach (var collection in CollectionByDate)
                CreateOrGetDateSnapshotInfo(collection.DateTime).Collection = collection.Info;

            foreach (var inventory in InventoryByDate)
                CreateOrGetDateSnapshotInfo(inventory.DateTime).Inventory = inventory.Info;

            foreach (var matches in MatchesByDate)
                CreateOrGetDateSnapshotInfo(matches.DateTime).Matches = matches.Info;

            foreach (var rankInfo in RankInfoByDate)
                CreateOrGetDateSnapshotInfo(rankInfo.DateTime).RankInfo = rankInfo.Info;

            foreach (var decks in DecksByDate)
                CreateOrGetDateSnapshotInfo(decks.DateTime).Decks = decks.Info;

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

        public InfoByDate<ICollection<ConfigModelRankInfo>> GetLastRank() => RankInfoByDate.OrderBy(i => i.DateTime).LastOrDefault()
            ?? new InfoByDate<ICollection<ConfigModelRankInfo>>(default(DateTime),
                new ConfigModelRankInfo[] {
                    new ConfigModelRankInfo(ConfigModelRankInfoFormatEnum.Constructed),
                    new ConfigModelRankInfo(ConfigModelRankInfoFormatEnum.Limited)
                });

        public InfoByDate<Inventory> GetLastInventory() => InventoryByDate.OrderBy(i => i.DateTime).LastOrDefault()
            ?? new InfoByDate<Inventory>(default(DateTime), new Inventory());

        public (DateSnapshotInfo info, DateSnapshotDiff diff) GetForDate(DateTime dateFor)
        {
            var resultForDate = new DateSnapshotInfo
            {
                Date = dateFor,
                Collection = CollectionByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Dictionary<int, int>(),
                Decks = DecksByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new ConfigModelRawDeck[0],
                Inventory = InventoryByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new Inventory(),
                Matches = MatchesByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new MatchResult[0],
                RankInfo = RankInfoByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new ConfigModelRankInfo[0],
            };

            var diff = DiffByDate.SingleOrDefault(i => i.DateTime.Date == dateFor)?.Info ?? new DateSnapshotDiff();

            return (resultForDate, diff);
        }
    }
}
