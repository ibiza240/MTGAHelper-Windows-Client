using System.Collections.Generic;
using MTGAHelper.Entity;

namespace MTGAHelper.Web.Models.Response.Misc
{
    public class GetCardsResponse
    {
        public ICollection<Card> Cards { get; }

        public GetCardsResponse(ICollection<Card> cards)
        {
            Cards = cards;
        }
    }
}
