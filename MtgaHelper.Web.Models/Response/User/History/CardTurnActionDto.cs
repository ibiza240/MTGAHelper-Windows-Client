using MTGAHelper.Web.UI.Model.SharedDto;

namespace MTGAHelper.Web.UI.Model.Response.User.History
{
    public class CardTurnActionDto
    {
        public int Turn { get; set; }
        public int Player { get; set; }
        public string Action { get; set; }
        public CardDto Card { get; set; }
    }
}
