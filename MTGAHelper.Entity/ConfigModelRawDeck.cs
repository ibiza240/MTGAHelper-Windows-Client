using System;
using System.Collections.Generic;

namespace MTGAHelper.Entity
{
    public class ConfigModelRawDeck
    {
        public string Id { get; set; }
        public int DeckTileId { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Name { get; set; }
        public string Format { get; set; }
        public string ArchetypeId { get; set; }
        public Dictionary<int, int> CardsMain { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> CardsSideboard { get; set; } = new Dictionary<int, int>();
    }
}
