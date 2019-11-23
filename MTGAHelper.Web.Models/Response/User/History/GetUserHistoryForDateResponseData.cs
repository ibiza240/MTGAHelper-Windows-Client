using AutoMapper;
using MTGAHelper.Lib.UserHistory;
using MTGAHelper.Web.Models.SharedDto;
using MTGAHelper.Web.UI.Model.Response.User.History;
using MTGAHelper.Web.UI.Model.SharedDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTGAHelper.Web.Models.Response.User.History
{
    public class GetUserHistoryForDateResponseData
    {
        public DateTime Date { get; set; }

        public GetUserHistoryForDateResponseDiff Diff { get; set; }
        public GetUserHistoryForDateResponseInfo Info { get; set; }

        // V2
        public ICollection<EconomyEventDto> EconomyEvents { get; set; }
        public ICollection<RankDeltaDto> RankUpdates { get; set; }
        public ICollection<MatchDto> Matches { get; set; }

        public GetUserHistoryForDateResponseData()
        {
        }

        public GetUserHistoryForDateResponseData(DateSnapshotInfo historyForDate, DateSnapshotDiff diff)
        {
            Date = historyForDate.Date;
            Info = Mapper.Map<GetUserHistoryForDateResponseInfo>(historyForDate);
            Diff = Mapper.Map<GetUserHistoryForDateResponseDiff>(diff);

            foreach (var m in Info.Matches.Where(i => i.DeckUsed == null))
                m.DeckUsed = new SimpleDeckDto();
        }
    }
}
