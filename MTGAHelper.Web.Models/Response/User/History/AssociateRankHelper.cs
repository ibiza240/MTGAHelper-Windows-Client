using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Entity.UserHistory;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using MTGAHelper.Web.Models.SharedDto;
using MTGAHelper.Web.UI.Model.Response.User.History;
using MTGAHelper.Web.UI.Model.SharedDto;

namespace MTGAHelper.Web.Models.Response.User.History
{
    public class AssociateRankHelper
    {
        public GetUserHistoryForDateResponseData LoadHistoryForDate(
            DateTime date,
            ICollection<MatchResult> matches,
            ICollection<EconomyEvent> economyEvents,
            ICollection<RankDelta> rankUpdates)
        {
            var history = new GetUserHistoryForDateResponseData(date, matches)
            {
                EconomyEvents = Mapper.Map<ICollection<EconomyEventDto>>(economyEvents),
                RankUpdates = Mapper.Map<ICollection<RankDeltaDto>>(rankUpdates),
            };

            // ASSOCIATE RANK CHANGES WITH MATCHES
            if (rankUpdates.Any())
            {
                foreach (var m in history.Matches)
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

            return history;
        }
    }
}
