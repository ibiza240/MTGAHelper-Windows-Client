namespace MTGAHelper.Web.Models.Response.Deck
{
    public class DeckFromUrlResponse
    {
        public string Name { get; set; }
        public string MtgaFormat { get; set; }

        public DeckFromUrlResponse(string name, string mtgaFormat)
        {
            Name = name;
            MtgaFormat = mtgaFormat;
        }
    }
}