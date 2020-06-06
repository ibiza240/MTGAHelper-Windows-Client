using System.Collections.Generic;
using MTGAHelper.Web.Models.SharedDto;

namespace MTGAHelper.Web.Models.Response.Misc
{
    public class GetCardsResponse
    {
        public ICollection<CardDtoFull> Cards { get; }

        public GetCardsResponse(ICollection<CardDtoFull> cards)
        {
            Cards = cards;
        }
    }
}
