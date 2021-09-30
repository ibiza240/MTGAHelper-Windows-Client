using MTGAHelper.Web.Models.SharedDto;

namespace MTGAHelper.Web.Models.Response.Deck
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