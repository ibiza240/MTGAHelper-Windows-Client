using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Entity.UserHistory;
using MTGAHelper.Lib.UserHistory;
using MTGAHelper.Web.Models.Response.User.History;
using MTGAHelper.Web.Models.SharedDto;
using MTGAHelper.Web.UI.Model.Response.User.History;
using MTGAHelper.Web.UI.Model.SharedDto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.Model.Response.User.History
{
    public class GetUserHistoryForDateResponse
    {
        DateTime dateNewHistory = new DateTime(2019, 11, 18);

        public GetUserHistoryForDateResponseData History { get; set; }
        public GetUserHistoryForDateResponseData History2 { get; set; }

        public GetUserHistoryForDateResponse(DateSnapshotInfo historyForDate, DateSnapshotDiff diff, ICollection<EconomyEvent> economyEvents, ICollection<RankDelta> rankUpdates)
        {
            History = new GetUserHistoryForDateResponseData(historyForDate, diff);

            var economyEventsDto = Mapper.Map<ICollection<EconomyEventDto>>(economyEvents);

            // V2
            if (historyForDate.Date < dateNewHistory)
            {
                if (History.Diff.NewCards.Count > 0)
                    economyEventsDto = new[]
                    {
                        new EconomyEventDto
                        {
                            DateTime = historyForDate.Date,
                            Context = "New cards",
                            NewCards = History.Diff.NewCards,
                        }
                    };
                else
                    economyEventsDto = new[]
                    {
                    new EconomyEventDto
                    {
                        DateTime = historyForDate.Date,
                        Context = "N/A",
                    }
                };
            }

            History2 = new GetUserHistoryForDateResponseData
            {
                EconomyEvents = economyEventsDto,
                RankUpdates = Mapper.Map<ICollection<RankDeltaDto>>(rankUpdates),
                Matches = Mapper.Map<ICollection<MatchDto>>(historyForDate.Matches),
            };

            // ASSOCIATE RANK CHANGES WITH MATCHES
            if (rankUpdates.Any())
            {
                foreach (var m in History2.Matches)
                {
                    var matchEnd = m.StartDateTime.AddSeconds(m.SecondsCount);

                    var bestRankUpdated = rankUpdates
                        .Select(i => new { timeDiff = Math.Abs((i.DateTime - matchEnd).TotalSeconds), i.DeltaSteps, i.RankEnd.Format })
                        .OrderBy(i => i.timeDiff)
                        .First();

                    if (bestRankUpdated.timeDiff < 5d)
                    {
                        m.RankDelta = new MatchRankDeltaDto
                        {
                            Format = bestRankUpdated.Format.ToString(),
                            StepsDelta = bestRankUpdated.DeltaSteps,
                        };
                    }


                    // Patch brought back from V1
                    if (m.DeckUsed == null)
                        m.DeckUsed = new SimpleDeckDto();
                }
            }
        }
    }
}
