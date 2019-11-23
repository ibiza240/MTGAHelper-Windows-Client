using System;
using System.Collections.Generic;

namespace MTGAHelper.Entity
{
    [Serializable]
    public class Card //: IEquatable<Card>
    {
        public const string NOTINBOOSTER = "NotInBooster";

        public string setAndInBooster => notInBooster ? NOTINBOOSTER : set;

        public ICollection<string> colors { get; set; } = new List<string>();
        public ICollection<string> color_identity { get; set; } = new List<string>();
        //public string layout { get; set; }
        public string name { get; set; }
        //public int mtgo_id { get; set; }
        public string number { get; set; }
        public string rarity { get; set; }
        public string mana_cost { get; set; }
        public string type { get; set; } = string.Empty;
        public string set { get; set; }
        public bool notInBooster { get; set; }
        //public bool craftedOnly { get; set; }
        public int grpId { get; set; }
        public string imageCardUrl { get; set; }
        public string imageArtUrl { get; set; }
        public int cmc { get; set; }
        //public string artist { get; set; }

        // From data_cards.mtga
        //public int titleId { get; set; }
        public enumLinkedFace linkedFaceType { get; set; }
        public bool isToken { get; set; }
        public bool isCraftable { get; set; }
        public bool isCollectible { get; set; }
        public string artistCredit { get; set; }
        
        //public string artistCredit { get; set; }

        public Card()
        {
            // For Serialization
        }

        //public Card(string set, Card c)
        //{
        //    this.set = set.ToUpper();

        //    //artist = c.artist;
        //    //cmc = c.cmc;
        //    colors = c.colors;
        //    //colors = c.colors;
        //    //flavor = c.flavor;
        //    //id = c.id;
        //    //imageName = c.imageName;
        //    layout = c.layout;
        //    //manaCost = c.manaCost;
        //    //mciNumber = c.mciNumber;
        //    //multiverseid = c.multiverseid;
        //    name = c.name;
        //    //names = c.names;
        //    number = c.number;
        //    //power = c.power;
        //    rarity = c.rarity;
        //    //subtypes = c.subtypes;
        //    //text = c.text;
        //    //toughness = c.toughness;
        //    type = c.type;
        //    //types = c.types;
        //}

        //public Card(Card c)
        //    : this(c.set, c)
        //{
        //}

        public string GetSimpleType()
        {
            var simpleType = "Land";

            if (type.Contains("Land") == false)
            {
                var t = type.Replace("Legendary", "").Trim();
                var typeWords = t.Contains("—") ? t.Substring(0, t.IndexOf("—")).Trim() : t;
                simpleType = typeWords;// string.Join("", typeWords.Split(' ').Select(i => i));
            }

            return simpleType.Split(new[] { "//" }, StringSplitOptions.None)[0].Trim();
        }
        public RarityEnum GetRarityEnum(bool splitRareLands = false)
        {
            // Because different sources of cards...
            // Scryfall: "Mythic Rare"
            // MTGATool: "mythic"
            var r = rarity.Split(' ')[0].ToLower();

            if (r == "rare")
            {
                if (splitRareLands)
                {
                    if (type.ToLower().Contains("land"))
                        return RarityEnum.RareLand;
                    else
                        return RarityEnum.RareNonLand;
                }
                else
                    return RarityEnum.Rare;
            }
            else
                switch (r)
                {
                    case "mythic":
                        return RarityEnum.Mythic;
                    case "uncommon":
                        return RarityEnum.Uncommon;
                    case "common":
                        return RarityEnum.Common;
                    default:
                        return RarityEnum.Unknown;
                }
        }

        public bool CompareNameWith(string n)
        {
            if (name == n)
            {
                return true;
            }
            // To match cards with variants ("Card name (a)")
            //return name.Length > 4 && name.Substring(0, name.Length - 4) == n;
            return false;
        }

        //public static bool operator ==(Card c1, Card c2)
        //{
        //    if (c1 is null || c2 is null)
        //    {
        //        return c1 is null && c2 is null;
        //    }

        //    return c1.Equals(c2);
        //}

        //public static bool operator !=(Card c1, Card c2)
        //{
        //    return !(c1 == c2);
        //}

        //public bool Equals(Card other)
        //{
        //    if (other is null)
        //    {
        //        return false;
        //    }

        //    if (ReferenceEquals(this, other))
        //        return true;
        //    else
        //        return CompareNameWith(other.name);
        //}

        public override int GetHashCode()
        {
            return -191684997 + EqualityComparer<string>.Default.GetHashCode(name);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override string ToString()
        {
            return name;
        }

        public bool IsEqualV2(Card other, bool isExact)
        {
            if (isExact)
            {
                return this.grpId == other.grpId;
            }
            else
            {
                return this == other;
            }
        }
    }
}
