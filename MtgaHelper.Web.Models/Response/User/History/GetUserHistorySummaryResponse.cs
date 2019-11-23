using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.UserHistory;
using MTGAHelper.Web.UI.Model.SharedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Lib;
using MTGAHelper.Entity.UserHistory;
using System.Text.RegularExpressions;
using MTGAHelper.Web.Models.SharedDto;

namespace MTGAHelper.Web.UI.Model.Response.User.History
{
    public class GetUserHistorySummaryResponse
    {
        Regex regex_Rank_StringParts = new Regex(@"^(.*?)_(.*?)_(.*?)$", RegexOptions.Compiled);

        public ICollection<GetUserHistorySummaryDto> History { get; set; }
        public ICollection<GetUserHistorySummaryDto> History2 { get; set; }

        public int TotalItems { get; set; }

        DateTime dateNewHistory = new DateTime(2019, 11, 18);

        //public GetUserHistoryResponse(ICollection<DateSnapshot> details)
        //{
        //    History = details
        //        .Select(i => Mapper.Map<GetUserHistoryDto>(i.Info).Map(i.Diff))
        //        .ToArray();
        //}

        public GetUserHistorySummaryResponse()
        {
        }

        public GetUserHistorySummaryResponse(HistorySummaryForDate[] summary, ICollection<HistorySummaryForDate> summary2, int currentPage, int perPage, int totalItems)
        {
            TotalItems = totalItems;

            History = summary
                .Select(i => Mapper.Map<GetUserHistorySummaryDto>(i))
                .ToArray();

            var history2 = summary2
                .Select(i => Mapper.Map<GetUserHistorySummaryDto>(i))
                .ToArray();

            // TEMP: Take "perPage" again because of the merge
            History2 = Merge(History.Skip(currentPage * perPage).Take(perPage).ToArray(), history2).Take(perPage).ToArray();
        }

        ICollection<GetUserHistorySummaryDto> Merge(ICollection<GetUserHistorySummaryDto> history, GetUserHistorySummaryDto[] history2)
        {
            var o = history.Where(i => i.Date.Date < dateNewHistory);
            foreach (var i in o)
            {
                var m = regex_Rank_StringParts.Match(i.ConstructedRank);
                i.ConstructedRankChange = new RankDeltaDto
                {
                    deltaSteps = 420,
                    RankEnd = new RankDto
                    {
                        Format = "Constructed",
                        Class = m.Groups[1].Value.ToString(),
                        Level = Convert.ToInt32(m.Groups[2].Value),
                        Step = Convert.ToInt32(m.Groups[3].Value),
                    }
                };

                m = regex_Rank_StringParts.Match(i.LimitedRank);
                i.LimitedRankChange = new RankDeltaDto
                {
                    deltaSteps = 420,
                    RankEnd = new RankDto
                    {
                        Format = "Limited",
                        Class = m.Groups[1].Value.ToString(),
                        Level = Convert.ToInt32(m.Groups[2].Value),
                        Step = Convert.ToInt32(m.Groups[3].Value),
                    }
                };
            }

            var n = history2.Where(i => i.Date.Date >= dateNewHistory);

            return o.Union(n).OrderByDescending(i => i.Date).ToArray();
        }
    }
}
