using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Web.UI.Model.SharedDto;
using System.Collections.Generic;

namespace MTGAHelper.Web.Models.Response.User
{
    public class CardForDraftPickDto : CardDto
    {
        public string Rating { get; set; }
        public string Description { get; set; }
        public float Weight { get; set; }
        public RaredraftPickReasonEnum IsRareDraftPick { get; set; }
        public string Rarity { get; set; }
        public int NbMissingTrackedDecks { get; set; }
        public int NbMissingCollection { get; set; }
        public int NbDecksUsedMain { get; set; }
        public int NbDecksUsedSideboard { get; set; }
        public DraftRatingTopCard TopCommonCard { get; set; }
        public ICollection<string> Colors { get; set; }
        public string ImageArtUrl { get; set; }
    }

    public class GetCardsForDraftPickResponse
    {
        public ICollection<CardForDraftPickDto> CardsForDraft { get; set; }

        public GetCardsForDraftPickResponse(ICollection<CardForDraftPick> data)
        {
            CardsForDraft = Mapper.Map<ICollection<CardForDraftPickDto>>(data);
        }
    }
}
