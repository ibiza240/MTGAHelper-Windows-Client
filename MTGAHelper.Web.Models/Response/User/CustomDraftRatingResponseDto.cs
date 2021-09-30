using MTGAHelper.Web.Models.SharedDto;

namespace MTGAHelper.Web.Models.Response.User
{
    public class CustomDraftRatingResponseDto
    {
        public CardDto Card { get; set; }
        public string Note { get; set; }
        public int? Rating { get; set; }
    }
}