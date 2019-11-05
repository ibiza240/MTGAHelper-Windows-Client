using MTGAHelper.Entity;
using System.Collections.Generic;

namespace MTGAHelper.Lib.UserHistory
{
    public class DateSnapshotDiff
    {
        public Dictionary<int, int> NewCards { get; set; } = new Dictionary<int, int>();
        public int GoldChange { get; set; }
        public int GemsChange { get; set; }
        public Dictionary<string, int> XpChangeByTrack { get; set; } = new Dictionary<string, int>();
        public float VaultProgressChange { get; set; }
        public Dictionary<RarityEnum, int> WildcardsChange { get; set; } = new Dictionary<RarityEnum, int>
        {
            { RarityEnum.Mythic, 0 },
            { RarityEnum.Rare, 0 },
            { RarityEnum.Uncommon, 0 },
            { RarityEnum.Common, 0 },
        };

        public DateSnapshotDiff()
        {
        }

        public DateSnapshotDiff(Dictionary<int, int> newCards)
        {
            NewCards = newCards;
        }
    }
}
