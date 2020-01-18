using System;
using System.Collections.Generic;
using MTGAHelper.Web.Models.SharedDto;

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

        public RankDeltaDto ConstructedRankChange { get; set; } = new RankDeltaDto();
        public RankDeltaDto LimitedRankChange { get; set; } = new RankDeltaDto();
    }
}
