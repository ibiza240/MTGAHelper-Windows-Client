using MTGAHelper.Web.UI.Model.SharedDto;
using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Response.User
{
    public class CustomDraftRatingResponseDto
    {
        public CardDto Card { get; set; }
        public string Note { get; set; }
        public int? Rating { get; set; }
    }
}
