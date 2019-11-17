using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    public class BoosterOpenedCard
    {
        public int GrpId { get; set; }
        public RarityEnum Rarity { get; set; }
        public bool IsWildcard { get; set; }
        public bool IsNew { get; set; }
    }

    public class BoosterOpened
    {
        public string Set { get; set; }
        public float VaultProgress { get; set; }
        public int Gems { get; set; }
        public RarityEnum WildcardFromTrack { get; set; }
        public ICollection<BoosterOpenedCard> Cards { get; set; }
    }
}
