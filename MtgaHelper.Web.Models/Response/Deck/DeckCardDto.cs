namespace MTGAHelper.Web.UI.Model.Response.Dto
{
    public class DeckCardDto
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public string Rarity { get; set; }
        public string Color { get; set; }
        public string ManaCost { get; set; }
        public string ImageCardUrl { get; set; }

        public string Type { get; set; }
        public int NbMissing { get; set; }
    }
}
