using System;
using System.Collections.Generic;

namespace MTGAHelper.Entity.UserHistory
{
    public class EconomyEvent
    {
        public string Context { get; set; }
        public DateTime DateTime { get; set; }
        public ICollection<EconomyEventChange> Changes { get; set; }
        public IReadOnlyDictionary<int, int> NewCards { get; set; } = new Dictionary<int, int>();
    }

    public class EconomyEventChange
    {
        public float Amount { get; set; }
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
        TokenSealed,
        TokenLimited,
    }
}