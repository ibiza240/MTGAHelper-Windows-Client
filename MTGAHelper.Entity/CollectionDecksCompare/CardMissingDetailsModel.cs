namespace MTGAHelper.Entity.CollectionDecksCompare
{
    public class CardMissingDetailsModel
    {
        public float MissingWeight { get; set; }
        public int NbOwned { get; set; }
        public int NbMissing { get; set; }
        public int NbDecks { get; set; }
        public double NbAvgPerDeck { get; set; }
        public string CardName { get; set; }
        //public string SetId { get; set; }
        public string Set { get; set; }
        public bool NotInBooster { get; set; }
        public RarityEnum Rarity { get; set; }
        public string Type { get; set; }
        public string ImageCardUrl { get; set; }
    }
}
