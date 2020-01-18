using AutoMapper;
using MTGAHelper.Lib;
using System.Collections.Generic;
using System.Linq;

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
        public bool NotInBooster { get; set; }
        public string Rarity { get; set; }
        public int NbMissing { get; set; }
        public float MissingWeight { get; set; }
    }

    public class DashboardResponse
    {
        public CardMissingDetailsModelResponseDto[] Details { get; set; }
        public KeyValuePair<string, InfoCardMissingSummaryResponseDto[]>[] Summary { get; set; }

        public DashboardResponse(DashboardModel model)
        {
            Details = Mapper.Map<CardMissingDetailsModelResponseDto[]>(model.Details);

            var summary = Mapper.Map<Dictionary<string, InfoCardMissingSummaryResponseDto[]>>(model.Summary);
            Summary = summary.Select(i => i).ToArray();
        }
    }
}
