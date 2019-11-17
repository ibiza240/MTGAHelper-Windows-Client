using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.UserHistory;
using MTGAHelper.Web.UI.Model.SharedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Lib;
using MTGAHelper.Entity.UserHistory;

namespace MTGAHelper.Web.UI.Model.Response.User.History
{
    public class GetUserHistorySummaryDto
    {
        public DateTime Date { get; set; }
        //public ICollection<CardWithAmountDto> NewCards { get; set; }
        public int NewCardsCount { get; set; }
        public int GoldChange { get; set; }
        public int GemsChange { get; set; }
        public int XpChange { get; set; }
        public ICollection<KeyValuePair<string, int>> BoostersChange { get; set; }
        public float VaultProgressChange { get; set; }
        public Dictionary<string, int> WildcardsChange { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }

        //public ICollection<MatchDto> Matches { get; set; }

        public string ConstructedRank { get; set; }
        public string LimitedRank { get; set; }
    }

    public class GetUserHistorySummaryResponse
    {
        public ICollection<GetUserHistorySummaryDto> History { get; set; }
        public ICollection<GetUserHistorySummaryDto> History2 { get; set; }

        //public GetUserHistoryResponse(ICollection<DateSnapshot> details)
        //{
        //    History = details
        //        .Select(i => Mapper.Map<GetUserHistoryDto>(i.Info).Map(i.Diff))
        //        .ToArray();
        //}

        public GetUserHistorySummaryResponse(HistorySummaryForDate[] summary, ICollection<HistorySummaryForDate> summary2)
        {
            History = summary
                .Select(i => Mapper.Map<GetUserHistorySummaryDto>(i))
                .ToArray();

            History2 = summary2
                .Select(i => Mapper.Map<GetUserHistorySummaryDto>(i))
                .ToArray();
        }
    }
}
