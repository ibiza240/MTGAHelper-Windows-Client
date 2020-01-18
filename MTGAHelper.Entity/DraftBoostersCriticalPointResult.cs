namespace MTGAHelper.Entity
{
    public class DraftBoostersCriticalPointResult
    {
        public int NbRaresMissing { get; set; }
        public int NbMythicsMissing { get; set; }

        /// <summary>Expected number of drafts still needed to complete
        /// before opening all packs results in a play set of every rare card in the set</summary>
        public float ExpectedNbDraftsToPlaysetRares { get; set; }

        /// <summary>Expected number of drafts still needed to complete
        /// before opening all packs results in a play set of every mythic rare card in the set</summary>
        public float ExpectedNbDraftsToPlaysetMythics { get; set; }

        ///// <summary>The chance that opening all current packs results in a full play set of rares</summary>
        //public double ChanceFullPlaysetRares { get; set; }

        ///// <summary>The chance that opening all current packs results in a full play set of mythics</summary>
        //public double ChanceFullPlaysetMythics { get; set; }

        /// <summary>The expected number of rare wildcards gained by opening current packs of the set.
        /// Does not take wildcard track into account!</summary>
        public float ExpectedRareWcsFromPacks { get; set; }

        /// <summary>The expected number of mythic wildcards gained by opening current packs of the set.
        /// Does not take wildcard track into account!</summary>
        public float ExpectedMythicWcsFromPacks => ExpectedRareWcsFromPacks;

        /// <summary>wildcard gain from opening current packs of the set</summary>
        public int RareWcsFromTrack { get; set; }

        /// <summary>wildcard gain from opening current packs of the set</summary>
        public int MythicWcsFromTrack { get; set; }

        /// <summary> total expected wildcard gain from opening current packs of the set</summary>
        public float ExpectedRareWcs => RareWcsFromTrack + ExpectedRareWcsFromPacks;

        /// <summary> total expected wildcard gain from opening current packs of the set</summary>
        public float ExpectedMythicWcs => MythicWcsFromTrack + ExpectedMythicWcsFromPacks;

        public DraftBoostersCriticalPointRarityInfo InfoRare { get; set; }
        public DraftBoostersCriticalPointRarityInfo InfoMythic { get; set; }
    }

    public class DraftBoostersCriticalPointRarityInfo
    {
        public int T { get; set; }
        public int R { get; set; }
        public int P { get; set; }
        public float N { get; set; }
        public float W { get; set; }
    }
}