using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    public enum InfoByDateKeyEnum
    {
        Unknown,
        Collection,
        Rank,
        Matches,
        Inventory,
        Decks,

        /// <summary>
        /// OBSOLETE, DO NOT USE
        /// </summary>
        Diff,

        PlayerProgress,
        PlayerProgressIntraday,
        PlayerQuests,
        InventoryUpdates,
        PostMatchUpdates,
        MtgaDecksFound,
        DraftPickProgress,
        DraftPickProgressIntraday,
        CrackedBoosters,
        VaultsOpened,
        CollectionIntraday,
        InventoryIntraday,
        RankUpdated,
        PayEntry,
        MythicRatingUpdated,
        CombinedRankInfo,
        EventClaimPrice,

        DatesWithData,
    }
}
