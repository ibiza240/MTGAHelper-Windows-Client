using System;

namespace MTGAHelper.Web.Models.SharedDto
{
    public class RankDto
    {
        public int SeasonOrdinal { get; set; }
        public string Format { get; set; }
        public string Class { get; set; }
        public int Level { get; set; }
        public int Step { get; set; }
    }

    public class RankDeltaDto
    {
        public DateTime DateTime { get; set; }
        public RankDto RankStart { get; set; }
        public RankDto RankEnd { get; set; }

        public int deltaSteps { get; set; }
    }
}
