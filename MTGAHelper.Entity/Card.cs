using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MTGAHelper.Entity
{
    public class Card //: IEquatable<Card>
    {
        public const string NOTINBOOSTER = "NotInBooster";

        private const string unknownImage =
            "https://cdn11.bigcommerce.com/s-0kvv9/images/stencil/1280x1280/products/266486/371622/classicmtgsleeves__43072.1532006814.jpg?c=2&imbypass=on";
        public static Card Unknown { get; } = new Card("Unknown", 0, unknownImage);


        public string setAndInBooster => notInBooster ? NOTINBOOSTER : set;

        public ICollection<string> colors { get; }
        public ICollection<string> color_identity { get; }
        //public string layout { get; set; }
        public string name { get; }
        //public int mtgo_id { get; set; }
        public string number { get; }
        public string rarity { get; }
        public string mana_cost { get; }
        public string type { get; }
        public string set { get; }
        public bool notInBooster { get; }
        //public bool craftedOnly { get; set; }
        public int grpId { get; }
        public string imageCardUrl { get; }
        public string imageArtUrl { get; }
        public int cmc { get; }
        //public string artist { get; set; }

        // From data_cards.mtga
        //public int titleId { get; set; }
        public enumLinkedFace linkedFaceType { get; }

        /// <summary>
        /// If applicable, the card face that would be found in the library or graveyard,
        /// or the other face of the card, if the current face would be found in the library.
        /// For split cards: only the halves (which are not found in the library) have this value set.
        /// </summary>
        public int LinkedCardGrpId { get; }

        public Card LinkedCard { get; private set; }

        public bool isToken { get; }
        //public bool isCraftable { get; set; }
        public bool isCollectible { get; }
        public string artistCredit { get; }


        private Card(string name, int grpId, string imageCardUrl, string type = null, ICollection<string> colors = null, ICollection<string> colorIdentity = null)
        {
            this.name = name;
            this.grpId = grpId;
            this.imageCardUrl = imageCardUrl;
            this.type = type ?? string.Empty;
            this.colors = colors ?? new string[0];
            this.color_identity = colorIdentity ?? new string[0];
        }

        public Card(Card2 card)
        : this(card.Name, card.GrpId, card.imageCardUrl, card.TypeLine, card.Colors, card.ColorIdentity)
        {
            number = card.Number;
            rarity = card.Rarity;
            mana_cost = card.ManaCost;
            set = card.SetScryfall.ToUpper();
            notInBooster = !card.IsInBooster;
            grpId = card.GrpId;
            imageArtUrl = card.imageArtUrl;
            cmc = card.Cmc;
            linkedFaceType = card.LinkedFaceType;
            LinkedCardGrpId = card.LinkedFaces?.Count == 1 ? card.LinkedFaces.First() : 0;
            isToken = card.IsToken;
            isCollectible = card.IsCollectible;
            artistCredit = card.Artist;
        }

        [JsonConstructor]// for use in ConsoleSync (maybe elsewhere?)
        public Card(string name, int grpId, string imageCardUrl, ICollection<string> colors, ICollection<string> colorIdentity, string number,
            string rarity, string manaCost, string type, string set, bool notInBooster, int cmc, enumLinkedFace linkedFaceType, int linkedCardGrpId,
            bool isToken, bool isCollectible, string artistCredit)
        : this(name, grpId, imageCardUrl, type, colors, colorIdentity)
        {
            this.number = number;
            this.rarity = rarity;
            this.mana_cost = manaCost;
            this.set = set;
            this.notInBooster = notInBooster;
            this.imageArtUrl = imageArtUrl;
            this.cmc = cmc;
            this.linkedFaceType = linkedFaceType;
            this.LinkedCardGrpId = linkedCardGrpId;
            this.isToken = isToken;
            this.isCollectible = isCollectible;
            this.artistCredit = artistCredit;
        }

        public void SetLinkedCard(Card card)
        {
            LinkedCard ??= card;
        }

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
            if (string.IsNullOrWhiteSpace(rarity))
                return RarityEnum.Unknown;

            // Because different sources of cards...
            // Scryfall: "Mythic Rare"
            // MTGATool: "mythic"
            var r = rarity.Split(' ')[0].ToLower();

            switch (r)
            {
                case "mythic":
                    return RarityEnum.Mythic;
                case "rare":
                    if (splitRareLands)
                        return type.ToLower().Contains("land") ? RarityEnum.RareLand : RarityEnum.RareNonLand;
                    return RarityEnum.Rare;
                case "uncommon":
                    return RarityEnum.Uncommon;
                case "common":
                    return RarityEnum.Common;
                default:
                    return RarityEnum.Unknown;
            }
        }

        public bool DoesProduceManaOfColor(string color)
        {
            return ThisFaceProducesManaOfColor(color) || linkedFaceType == enumLinkedFace.MDFC_Front && (LinkedCard?.ThisFaceProducesManaOfColor(color) == true);
        }

        public bool ThisFaceProducesManaOfColor(string color)
        {
            return type.Contains("Land") && (color == "C" ? color_identity.Count == 0 : color_identity.Contains(color));
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
    }
}
