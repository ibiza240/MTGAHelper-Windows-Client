using AutoMapper;
using MTGAHelper.Lib;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Entity;

namespace MTGAHelper.Web.UI.Model.Response
{
    public class CardMissingDetailsModelResponseDto
    {
        public float MissingWeight { get; set; }
        public int NbOwned { get; set; }
        public int NbMissing { get; set; }
        public int NbDecks { get; set; }
        public double NbAvgPerDeck { get; set; }
        public string CardName { get; set; }
        //public string SetId { get; set; }
        public string Set { get; set; }
        public bool NotInBooster { get; set; }
        public string Rarity { get; set; }
        public string Type { get; set; }
        public string ImageCardUrl { get; set; }
    }

    public class InfoCardMissingSummaryResponseDto
    {
        public string Set { get; set; }
        public string Rarity { get; set; }
        public int NbMissing { get; set; }
        public float MissingWeight { get; set; }
    }

    public class DashboardResponse
    {
        public CardMissingDetailsModelResponseDto[] Details { get; }
        public KeyValuePair<string, InfoCardMissingSummaryResponseDto[]>[] Summary { get; }

        public DashboardResponse(CardMissingDetailsModelResponseDto[] details, KeyValuePair<string, InfoCardMissingSummaryResponseDto[]>[] summary)
        {
            Details = details;
            Summary = summary;
        }

        public static DashboardResponse FromModel(DashboardModel model, IMapper mapper)
        {
            var details = mapper.Map<CardMissingDetailsModelResponseDto[]>(model.Details);
            var summary = mapper.Map<Dictionary<string, InfoCardMissingSummaryResponseDto[]>>(model.Summary);
            return new DashboardResponse(details, summary.ToArray());
        }
    }
}
