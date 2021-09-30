namespace MTGAHelper.Web.Models.SharedDto
{
    public class CardDto
    {
        public string Set { get; set; }
        public int IdArena { get; set; }
        public string Name { get; set; }
        public string ImageCardUrl { get; set; }
    }

    public class CardWithAmountDto : CardDto
    {
        public int Amount { get; set; }
        public string Rarity { get; set; }
        public string Color { get; set; }
    }

    public class CollectionCardDto : CardWithAmountDto
    {
        public bool NotInBooster { get; set; }
    }
}