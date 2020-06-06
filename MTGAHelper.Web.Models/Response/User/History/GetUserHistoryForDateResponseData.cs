using System;
using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Web.Models.SharedDto;
using MTGAHelper.Web.UI.Model.Response.User.History;
using MTGAHelper.Web.UI.Model.SharedDto;

namespace MTGAHelper.Web.Models.Response.User.History
{
    public class GetUserHistoryForDateResponseData
    {
        public DateTime Date { get; }

        public ICollection<MatchDto> Matches { get; }
        public ICollection<EconomyEventDto> EconomyEvents { get; }
        public ICollection<RankDeltaDto> RankUpdates { get; }

        public GetUserHistoryForDateResponseData(DateTime date, ICollection<MatchDto> matches, ICollection<EconomyEventDto> economyEvents, ICollection<RankDeltaDto> rankUpdates)
        {
            Date = date;
            Matches = matches;
            EconomyEvents = economyEvents;
            RankUpdates = rankUpdates;

            foreach (var m in Matches.Where(i => i.DeckUsed == null))
                m.DeckUsed = new SimpleDeckDto();
        }
    }
}
