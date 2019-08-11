using System;
using System.Collections.Generic;
using System.Text;
using MTGAHelper.Entity;

namespace MTGAHelper.Tracker.WPF.Models
{
    public class Card
    {
        public int ArenaId { get; set; }
        public string Name { get; set; }
        public string Rarity { get; set; }
        public string ImageCardUrl { get; set; }
        public string ImageArtUrl { get; set; }
        public ICollection<string> Colors { get; set; }
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
        public DraftRatingTopCard TopCommonCard { get; set; }

        public float RatingFloat => float.TryParse(Rating.Substring(0, 3), out float f) ? f : 0f;

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
