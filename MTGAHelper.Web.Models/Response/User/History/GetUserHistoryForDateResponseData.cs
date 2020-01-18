using AutoMapper;
using MTGAHelper.Lib.IO.Reader.MtgaOutputLog;
using MTGAHelper.Web.Models.SharedDto;
using MTGAHelper.Web.UI.Model.Response.User.History;
using MTGAHelper.Web.UI.Model.SharedDto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGAHelper.Web.Models.Response.User.History
{
    public class GetUserHistoryForDateResponseData
    {
        public DateTime Date { get; set; }

        public ICollection<EconomyEventDto> EconomyEvents { get; set; }
        public ICollection<RankDeltaDto> RankUpdates { get; set; }
        public ICollection<MatchDto> Matches { get; set; }

        public GetUserHistoryForDateResponseData(DateTime date, ICollection<MatchResult> matches)
        {
            Date = date;
            Matches = Mapper.Map<ICollection<MatchDto>>(matches);

            foreach (var m in Matches.Where(i => i.DeckUsed == null))
                m.DeckUsed = new SimpleDeckDto();
        }
    }

    public class OldGetUserHistoryForDateResponseData
    {
        public DateTime Date { get; set; }

        //public GetUserHistoryForDateResponseDiff Diff { get; set; }
        //public GetUserHistoryForDateResponseInfo Info { get; set; }

        // V2
        public ICollection<EconomyEventDto> EconomyEvents { get; set; }
        public ICollection<RankDeltaDto> RankUpdates { get; set; }
        public ICollection<MatchDto> Matches { get; set; }

        //public OldGetUserHistoryForDateResponseData()
        //{
        //}

        //public OldGetUserHistoryForDateResponseData(DateSnapshotInfo historyForDate, DateSnapshotDiff diff)
        //{
        //    Date = historyForDate.Date;
        //    Info = Mapper.Map<GetUserHistoryForDateResponseInfo>(historyForDate);
        //    Diff = Mapper.Map<GetUserHistoryForDateResponseDiff>(diff);

        //    foreach (var m in Info.Matches.Where(i => i.DeckUsed == null))
        //        m.DeckUsed = new SimpleDeckDto();
        //}
    }
}
