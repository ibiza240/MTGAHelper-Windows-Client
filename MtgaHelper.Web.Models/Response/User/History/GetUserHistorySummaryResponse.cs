using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.UserHistory;
using MTGAHelper.Web.UI.Model.SharedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Lib;

namespace MTGAHelper.Web.UI.Model.Response.User.History
{
    public class GetUserHistorySummaryDto
    {
        public DateTime Date { get; set; }
        //public ICollection<CardWithAmountDto> NewCards { get; set; }
        public int NewCardsCount { get; set; }
        public int GoldChange { get; set; }
        public int GemsChange { get; set; }
        public float VaultProgressChange { get; set; }
        public Dictionary<RarityEnum, int> WildcardsChange { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

        //public ICollection<MatchDto> Matches { get; set; }

        public string ConstructedRank { get; set; }
        public string LimitedRank { get; set; }
    }

    public class GetUserHistorySummaryResponse
    {
        public ICollection<GetUserHistorySummaryDto> History { get; set; }

        //public GetUserHistoryResponse(ICollection<DateSnapshot> details)
        //{
        //    History = details
        //        .Select(i => Mapper.Map<GetUserHistoryDto>(i.Info).Map(i.Diff))
        //        .ToArray();
        //}

        public GetUserHistorySummaryResponse(ICollection<HistorySummaryForDate> summary)
        {
            History = summary
                .Select(i => Mapper.Map<GetUserHistorySummaryDto>(i))
                .ToArray();
        }
    }
}
