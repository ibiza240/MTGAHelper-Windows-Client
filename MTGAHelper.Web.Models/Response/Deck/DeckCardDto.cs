using MTGAHelper.Web.UI.Model.SharedDto;

namespace MTGAHelper.Web.UI.Model.Response.Dto
{
    public class DeckCardDto : CardWithAmountDto
    {
        public string Zone { get; set; }
        public string ManaCost { get; set; }
        public int Cmc { get; set; }
        public string ImageThumbnail { get; set; }

        public string Type { get; set; }
        public int NbMissing { get; set; }
    }
}
