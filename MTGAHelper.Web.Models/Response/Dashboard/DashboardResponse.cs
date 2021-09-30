using AutoMapper;
using MTGAHelper.Lib;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Entity;

namespace MTGAHelper.Web.Models.Response.Dashboard
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

    public class DashboardModelSummaryDto
    {
        public ICollection<InfoCardMissingSummaryResponseDto> Data { get; set; }
        public float ExpectedValue { get; set; }
        public KeyValuePair<string, float> ExpectedValueOther { get; set; }
    }

    //public class DashboardResponse
    //{
    //    public CardMissingDetailsModelResponseDto[] Details { get; }
    //    public KeyValuePair<string, DashboardModelSummaryDto>[] Summary { get; }

    //    public DashboardResponse(CardMissingDetailsModelResponseDto[] details, KeyValuePair<string, DashboardModelSummaryDto>[] summary)
    //    {
    //        Details = details;
    //        Summary = summary;
    //    }

    //    public static DashboardResponse FromModel(DashboardModel model, IMapper mapper)
    //    {
    //        var details = mapper.Map<CardMissingDetailsModelResponseDto[]>(model.Details);
    //        var summary = mapper.Map<Dictionary<string, DashboardModelSummaryDto>>(model.Summary);
    //        return new DashboardResponse(details, summary.ToArray());
    //    }
    //}
}