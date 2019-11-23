using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity.UserHistory
{
    public class EconomyEvent
    {
        public string Context { get; set; }
        public DateTime DateTime { get; set; }
        public ICollection<EconomyEventChange> Changes { get; set; }
        public Dictionary<int, int> NewCards { get; set; } = new Dictionary<int, int>();
    }

    public class EconomyEventChange
    {
        public dynamic Amount { get; set; }
        public EconomyEventChangeEnum Type { get; set; }
        public string Value { get; set; }
    }

    public enum EconomyEventChangeEnum
    {
        Unknown,
        NewCards,
        Gold,
        Gems,
        Vault,
        Xp,
        Wildcard,
        Booster,
    }
}
