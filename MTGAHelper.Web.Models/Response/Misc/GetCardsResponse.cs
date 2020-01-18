using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Web.Models.SharedDto;
using System.Collections.Generic;

namespace MTGAHelper.Web.Models.Response.Misc
{
    public class GetCardsResponse
    {
        public ICollection<CardDtoFull> Cards { get; set; }

        public GetCardsResponse()
        {
        }

        public GetCardsResponse(ICollection<Card> cards)
        {
            Cards = Mapper.Map<ICollection<CardDtoFull>>(cards);
        }
    }
}
