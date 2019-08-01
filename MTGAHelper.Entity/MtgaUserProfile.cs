using System.Collections.Generic;

namespace MTGAHelper.Entity
{
    public class Inventory
    {
        public string PlayerId { get; set; }

        public int Gold { get; set; }

        public int Gems { get; set; }

        public float VaultProgress { get; set; }

        public Dictionary<RarityEnum, int> Wildcards { get; set; } = new Dictionary<RarityEnum, int>
        {
            {  RarityEnum.Mythic, 0 },
            {  RarityEnum.Rare, 0 },
            {  RarityEnum.Uncommon, 0 },
            {  RarityEnum.Common,0 },
        };

        //public MtgaUserProfile()
        //{
        //}

        //public MtgaUserProfile(string playerId, int gold, int gems, float vaultProgress, int wcC, int wcU, int wcR, int wcM)
        //{
        //    PlayerId = playerId;
        //    Gold = gold;
        //    Gems = gems;
        //    VaultProgress = vaultProgress;
        //    Wildcards[RarityEnum.Common] = wcC;
        //    Wildcards[RarityEnum.Uncommon] = wcU;
        //    Wildcards[RarityEnum.Rare] = wcR;
        //    Wildcards[RarityEnum.Mythic] = wcM;
        //}
    }
}
