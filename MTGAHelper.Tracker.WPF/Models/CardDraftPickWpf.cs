using MTGAHelper.Entity;

namespace MTGAHelper.Tracker.WPF.Models
{
    public class CardDraftPickWpf : CardWpf
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
        public string RatingSource { get; set; }
        public DraftRatingTopCard TopCommonCard { get; set; } = new DraftRatingTopCard(0, "");

        public float RatingFloat => DraftRating.GetRatingAsFloat(Rating);
        public float RatingScale => DraftRating.GetRatingScale(RatingSource);

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
