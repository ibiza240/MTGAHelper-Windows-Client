using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MTGAHelper.Entity;

namespace MTGAHelper.Tracker.WPF.Models
{
    public class CardWithAmount : Card
    {
        public int Amount { get; set; }
    }

    public class Card
    {
        public int ArenaId { get; set; }
        public string Name { get; set; }
        public string Rarity { get; set; }
        public string ImageCardUrl { get; set; }
        public string ImageArtUrl { get; set; }
        public ICollection<string> Colors { get; set; } = new string[0];
        public string ManaCost { get; set; }
        public int Cmc { get; set; }
        public string Type { get; set; }

        static Regex regexCmcImages = new Regex(@"{([^}]+)}", RegexOptions.Compiled);

        public ICollection<string> CmcImages
        {
            get
            {
                var matches = regexCmcImages.Matches(ManaCost);
                if (matches.Count == 0) return new string[0];

                var ret = matches
                    .Select(i => i.Value.Replace("{", "").Replace("}", "").Replace("/", ""))
                    .Select(i => $"https://www.mtgahelper.com/images/manaIcons/{i}.png")
                    .ToArray();

                return ret;
            }
        }
    }

    public class CardDraftPick : Card
    {
        public int NbMissing { get; set; }
        public float Weight { get; set; }
        public int NbDecksUsedMain { get; set; }
        public int NbDecksUsedSideboard { get; set; }
        public RaredraftPickReasonEnum RareDraftPickEnum { get; set; }
        public bool IsRequiredToShowNbMissing => RareDraftPickEnum != RaredraftPickReasonEnum.MissingInCollection;
        public int NbMissingTrackedDecks { get; set; }
        public int NbMissingCollection { get; set; }
        public string NbMissingString => $"You are missing {NbMissingCollection} {(NbMissingCollection == 1 ? "copy" : "copies")} in your collection";

        public string Description { get; set; }
        public string Rating { get; set; }
        public DraftRatingTopCard TopCommonCard { get; set; } = new DraftRatingTopCard(0, "");

        public float RatingFloat => float.TryParse(Rating.Substring(0, 3), out float f) ? f : 0f;

        //public bool ShowMtgaHelperSays => Weight > 0 || RareDraftPickEnum != Entity.RaredraftPickReasonEnum.None;
        public string NbDecksUsedInfo => $"Played in {NbDecksUsedMain} tracked deck{(NbDecksUsedMain == 1 ? "" : "s")} and {NbDecksUsedSideboard} sideboard{(NbDecksUsedSideboard == 1 ? "" : "s")}";

        public string RareDraftPickReason
        {
            get
            {
                switch (RareDraftPickEnum)
                {
                    case RaredraftPickReasonEnum.RareLandMissing:
                        return $"Rare land!";
                    case RaredraftPickReasonEnum.HighestWeight:
                        return $"Highest priority based on your tracked decks";
                    case RaredraftPickReasonEnum.MissingInCollection:
                        return NbMissingString;
                    case RaredraftPickReasonEnum.BestVaultRarity:
                        return $"You own a playset of all these cards so this is for highest Vault progression value (Any {Rarity.Substring(0, 1).ToUpper() + Rarity.Substring(1, Rarity.Length - 1)})";
                    default:
                        return "N/A";
                }
            }
        }
    }
}
