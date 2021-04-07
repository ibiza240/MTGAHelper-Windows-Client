using MTGAHelper.Entity.OutputLogParsing;
using MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger;
using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.Models
{
    public class OutputLogResult2
    {
        public ICollection<ConverterUsage> LogReadersUsage { get; set; } = new ConverterUsage[0];

        public Dictionary<string, OutputLogResult2ByNameTag> ResultsByNameTag { get; set; } = new Dictionary<string, OutputLogResult2ByNameTag>()
        {
            [""] = new OutputLogResult2ByNameTag()
        };

        //public int GetResultsHash()
        //{
        //    var json = JsonConvert.SerializeObject(ResultsByNameTag);
        //    return json.GetHashCode();
        //}
    }

    public class OutputLogResult2ByNameTag
    {
        #region Result message is already a list, we only keep the latest
        public GetDecksListResult GetDecksListResults { get; set; } = new GetDecksListResult { Raw = new PayloadRaw<ICollection<CourseDeckRaw>> { payload = new List<CourseDeckRaw>() } };
        public GetPreconDecksV3Result GetPreconDecksV3Results { get; set; } = new GetPreconDecksV3Result { Raw = new PayloadRaw<ICollection<CourseDeckRaw>> { payload = new List<CourseDeckRaw>() } };
        public GetActiveEventsV3Result ActiveEvents { get; set; } = new GetActiveEventsV3Result { Raw = new PayloadRaw<ICollection<GetActiveEventsV3Raw>> { payload = new List<GetActiveEventsV3Raw>() } };
        #endregion

        public List<PlayerNameResult> PlayerNameResults { get; set; } = new List<PlayerNameResult>();
        public List<GetSeasonAndRankDetailResult> GetSeasonAndRankDetailResults { get; set; } = new List<GetSeasonAndRankDetailResult>();
        public List<RankUpdatedResult> RankUpdatedResults { get; set; } = new List<RankUpdatedResult>();
        //public List<MythicRatingUpdatedResult> MythicRatingUpdatedResults { get; set; } = new List<MythicRatingUpdatedResult>();
        public List<GetPlayerCardsResult> GetPlayerCardsResults { get; set; } = new List<GetPlayerCardsResult>();
        public List<GetPlayerProgressResult> GetPlayerProgressResults { get; set; } = new List<GetPlayerProgressResult>();
        public List<GetCombinedRankInfoResult> GetCombinedRankInfoResults { get; set; } = new List<GetCombinedRankInfoResult>();
        public List<GetPlayerQuestsResult> GetPlayerQuestsResults { get; set; } = new List<GetPlayerQuestsResult>();
        public List<GetEventPlayerCourseV2Result> GetEventPlayerCourseV2Results { get; set; } = new List<GetEventPlayerCourseV2Result>();
        public List<DraftResultBase> ResultDraftPicks { get; set; } = new List<DraftResultBase>();
        public List<EventClaimPrizeResult> EventClaimPrizeResults { get; set; } = new List<EventClaimPrizeResult>();
        //public List<PostMatchUpdateResult> PostMatchUpdateResults { get; set; } = new List<PostMatchUpdateResult>();
        public List<InventoryUpdatedResult> InventoryUpdatedResults { get; set; } = new List<InventoryUpdatedResult>();
        //public List<PayEntryResult> PayEntryResults { get; set; } = new List<PayEntryResult>();
        //public List<CompleteVaultResult> CompleteVaultResults { get; set; } = new List<CompleteVaultResult>();
        //public List<CrackBoostersResult> CrackBoostersResults { get; set; } = new List<CrackBoostersResult>();
        public List<GetPlayerInventoryResult> GetPlayerInventoryResults { get; set; } = new List<GetPlayerInventoryResult>();
    }
}
