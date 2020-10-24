﻿namespace MTGAHelper.Entity
{
    public class CardWithAmount
    {
        public Card Card { get; set; }
        public int Amount { get; set; }

        public CardWithAmount()
        {
            // Required for serialization
        }

        public CardWithAmount(Card card, int amount)
        {
            Card = card;
            Amount = amount;
        }

        public string DisplayMember { get { return $"{Amount}x {Card.name}"; } }

        public override string ToString()
        {
            return $"{Amount}x {Card.name}";
        }
    }

    public enum RaredraftPickReasonEnum
    {
        None,
        BestVaultRarity,
        HighestWeight,
        RareLandMissing,
        MissingInCollection,
    }

    public class CardForDraftPick : Card
    {
        public string RatingSource { get; set; } = "";
        public float RatingValue { get; set; } = 0f;
        public string RatingToDisplay { get; set; } = "N/A";
        public string Description { get; set; } = "N/A";
        public float Weight { get; set; }
        public int NbDecksUsedMain { get; set; }
        public int NbDecksUsedSideboard { get; set; }
        public RaredraftPickReasonEnum IsRareDraftPick { get; set; }
        public int NbMissingTrackedDecks { get; set; }
        public int NbMissingCollection { get; set; }
        public DraftRatingTopCard TopCommonCard { get; set; } = new DraftRatingTopCard(0, "");
    }
}
