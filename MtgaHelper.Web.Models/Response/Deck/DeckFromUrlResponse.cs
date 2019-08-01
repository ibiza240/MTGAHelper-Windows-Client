namespace MTGAHelper.Web.UI.Model.Response
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
