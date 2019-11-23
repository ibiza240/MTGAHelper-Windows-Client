using System.Collections.Generic;

namespace MTGAHelper.Entity
{
    public class InventoryBooster
    {
        public int CollationId { get; set; }
        public int Count { get; set; }
    }

    public class Inventory
    {
        public int Gold { get; set; }
        public int Gems { get; set; }
        public int Xp { get; set; }
        public int DraftTokens { get; set; }
        public int SealedTokens { get; set; }
        public int WcTrackPosition { get; set; }
        public float VaultProgress { get; set; }

        public ICollection<InventoryBooster> Boosters { get; set; } = new InventoryBooster[0];

        public Dictionary<RarityEnum, int> Wildcards { get; set; } = new Dictionary<RarityEnum, int>
        {
            {  RarityEnum.Mythic, 0 },
            {  RarityEnum.Rare, 0 },
            {  RarityEnum.Uncommon, 0 },
            {  RarityEnum.Common,0 },
        };
    }
}
