using MTGAHelper.Web.UI.Model.SharedDto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.UI.Model.Response.User.History
{
    public class MatchDtoLightweight
    {
        public string MatchId { get; set; }
        public string EventName { get; set; }
        public string OpponentName { get; set; }
        public string OpponentRank { get; set; }
        public long SecondsCount { get; set; }
        public string Outcome { get; set; }
        public DateTime StartDateTime { get; set; }
        public string OpponentDeckColors { get; set; }
        public string FirstTurn { get; set; }
    }

    public class MatchDto : MatchDtoLightweight
    {
        public SimpleDeckDto DeckUsed { get; set; }
        public ICollection<GameDetailDto> Games { get; set; }
    }
}
