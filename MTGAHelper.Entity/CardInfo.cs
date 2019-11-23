using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace MTGAHelper.Entity
{
    public class InfoCardMissingSummary
    {
        public string Set { get; set; }
        //public bool NotInBooster { get; set; }
        public RarityEnum Rarity { get; set; }
        public int NbMissing { get; set; }
        public float MissingWeight { get; set; }
    }

    public class InfoCardInDeck
    {
        public string Set { get; set; }
        public string CardName { get; set; }
        public string Rarity { get; set; }
        public string Type { get; set; }
        public string Deck { get; set; }
        public int NbMissingMain { get; set; }
        public int NbMissingSideboard { get; set; }
    }
}
