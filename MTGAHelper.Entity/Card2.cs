using System;
using System.Collections.Generic;

namespace MTGAHelper.Entity
{
    [Serializable]
    public class Card2
    {
        // from MTGA
        public int GrpId { get; set; }
        public string Name { get; set; }
        public bool IsToken { get; set; }
        public bool IsCollectible { get; set; }
        //public bool IsCraftable { get; set; }
        public int Power { get; set; }
        public int Toughness { get; set; }
        public string Number { get; set; }  // MTGA: CollectorNumber    MUST BE STRING (e.g. Golgari Queen GR8
        public int Cmc { get; set; }
        public string Rarity { get; set; }
        public enumLinkedFace LinkedFaceType { get; set; }
        public int LinkedCardGrpId { get; set; }

        // From MTGA but with custom logic
        public string SetScryfall { get; set; }

        //// from Scryfall
        //public ICollection<string> Colors { get; set; }
        //public ICollection<string> Color_identity { get; set; }
        //public string Type_line { get; set; }
        //public string Mana_cost { get; set; }

        // from Scryfall, calculated 
        public string imageCardUrl { get; set; }
        public string imageArtUrl { get; set; }

        // Scryfall overwrites this, when available
        public string Artist { get; set; }  // MTGA: ArtistCredit

        // Custom
        public bool IsInBooster { get; set; } = true;

        // New Cards format
        public ICollection<string> Colors { get; set; }
        public ICollection<string> ColorIdentity { get; set; }
        public string TypeLine { get; set; }
        public string ManaCost { get; set; }
    }
}
