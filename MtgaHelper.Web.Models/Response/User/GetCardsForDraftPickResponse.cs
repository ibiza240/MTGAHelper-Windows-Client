using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Web.UI.Model.SharedDto;

namespace MTGAHelper.Web.Models.Response.User
{
    public class CardForDraftPickDto : CardDto
    {
        public float Weight { get; set; }
        public string Rating { get; set; }
        public string Description { get; set; }
        public int Top5Rank { get; set; }
    }

    public class GetCardsForDraftPickResponse
    {
        public ICollection<CardForDraftPickDto> CardsForDraft { get; set; }

        public GetCardsForDraftPickResponse(ICollection<CardForDraftPick> data)
        {
            this.CardsForDraft = Mapper.Map<ICollection<CardForDraftPickDto>>(data);
        }
    }
}
