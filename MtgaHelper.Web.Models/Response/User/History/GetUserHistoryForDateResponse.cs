using AutoMapper;
using MTGAHelper.Entity;
using MTGAHelper.Lib.UserHistory;
using MTGAHelper.Web.UI.Model.SharedDto;
using System;
using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Response.User.History
{
    public class GetUserHistoryForDateResponse
    {
        public GetUserHistoryForDateResponseData History { get; set; }

        public GetUserHistoryForDateResponse(DateSnapshotInfo historyForDate, DateSnapshotDiff diff)
        {
            History = new GetUserHistoryForDateResponseData(historyForDate, diff);
        }
    }

    public class GetUserHistoryForDateResponseData
    {
        public DateTime Date { get; set; }

        public GetUserHistoryForDateResponseDiff Diff { get; set; }
        public GetUserHistoryForDateResponseInfo Info { get; set; }

        public GetUserHistoryForDateResponseData()
        {

        }

        public GetUserHistoryForDateResponseData(DateSnapshotInfo historyForDate, DateSnapshotDiff diff)
        {
            Date = historyForDate.Date;
            Info = Mapper.Map<GetUserHistoryForDateResponseInfo>(historyForDate);
            Diff = Mapper.Map<GetUserHistoryForDateResponseDiff>(diff);
        }
    }

    public class GetUserHistoryForDateResponseDiff
    {
        public ICollection<CardWithAmountDto> NewCards { get; set; }
        public int GoldChange { get; set; }
        public int GemsChange { get; set; }
        public float VaultProgressChange { get; set; }
        public Dictionary<RarityEnum, int> WildcardsChange { get; set; }
    }

    public class GetUserHistoryForDateResponseInfo
    {
        public ICollection<MatchDto> Matches { get; set; }
        public int Gold { get; set; }
        public int Gems { get; set; }
        public float VaultProgress { get; set; }
        public Dictionary<RarityEnum, int> Wildcards { get; set; }

        public RankInfoDto ConstructedRank { get; set; }
        public RankInfoDto LimitedRank { get; set; }
    }
}
