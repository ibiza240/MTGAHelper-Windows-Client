using System;
using System.Collections.Generic;

namespace MTGAHelper.Entity
{
    public class DeckTrackedSummary : DeckSummary
    {
        public float MissingWeight { get; set; }
        public float MissingWeightBase { get; set; }
        public float PriorityFactor { get; set; }
        public Dictionary<RarityEnum, int> WildcardsMissingMain { get; set; }
        public Dictionary<RarityEnum, int> WildcardsMissingSideboard { get; set; }
    }

    public class DeckSummary
    {
        public string Id { get; set; }
        public uint Hash { get; set; }
        public string Name { get; set; }
        public string ScraperTypeId { get; set; }
        public string Color { get; set; }
        public string Url { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
