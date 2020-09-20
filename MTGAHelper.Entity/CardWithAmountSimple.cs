namespace MTGAHelper.Entity
{
    public class CardWithAmountSimple
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public string ImageCardUrl { get; set; }

        public CardWithAmountSimple()
        {
        }

        public CardWithAmountSimple(int amount, string name, string imageCardUrl)
        {
            Amount = amount;
            Name = name;
            ImageCardUrl = imageCardUrl;
        }
    }
}
