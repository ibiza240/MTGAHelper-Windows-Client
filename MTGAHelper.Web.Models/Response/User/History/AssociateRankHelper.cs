using System;
using System.Linq;
using MTGAHelper.Web.UI.Model.Response.User.History;
using MTGAHelper.Web.UI.Model.SharedDto;

namespace MTGAHelper.Web.Models.Response.User.History
{
    public class AssociateRankHelper
    {
        public GetUserHistoryForDateResponseData AssociateRankWithMatches(
            GetUserHistoryForDateResponseData history)
        {
            // ASSOCIATE RANK CHANGES WITH MATCHES
            if (history.RankUpdates.Any())
            {
                foreach (var m in history.Matches)
                {
                    var matchEnd = m.StartDateTime.AddSeconds(m.SecondsCount);

                    var bestRankUpdated = history.RankUpdates
                        .Select(i => new { timeDiff = Math.Abs((i.DateTime - matchEnd).TotalSeconds), i.deltaSteps, i.RankEnd.Format })
                        .OrderBy(i => i.timeDiff)
                        .First();

                    if (bestRankUpdated.timeDiff < 5d)
                    {
                        m.RankDelta = new MatchRankDeltaDto
                        {
                            Format = bestRankUpdated.Format,
                            StepsDelta = bestRankUpdated.deltaSteps,
                        };
                    }


                    // Patch brought back from V1
                    m.DeckUsed ??= new SimpleDeckDto();
                }
            }

            return history;
        }
    }
}
