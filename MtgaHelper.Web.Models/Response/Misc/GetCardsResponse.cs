using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Web.Models.SharedDto;
using MTGAHelper.Web.UI.Model.SharedDto;
using System;
using System.Collections.Generic;
using System.Text;

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
