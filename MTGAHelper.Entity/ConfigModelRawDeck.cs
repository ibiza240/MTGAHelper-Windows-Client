using System.Collections.Generic;

namespace MTGAHelper.Entity
{
    public class ConfigModelRawDeck
    {
        public string Name { get; set; }
        public Dictionary<int, int> CardsMain { get; set; }
        public Dictionary<int, int> CardsSideboard { get; set; }
    }
}
