using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    public enum RankFormatEnum
    {
        Unknown,
        Constructed,
        Limited
    }

    public class Rank
    {
        public int SeasonOrdinal { get; set; }
        public RankFormatEnum Format { get; set; }
        public string Class { get; set; }
        public int Level { get; set; }
        public int Step { get; set; }
    }

    public class RankDelta
    {
        public DateTime DateTime { get; set; }
        public Rank RankStart { get; set; }
        public Rank RankEnd { get; set; }

        public int DeltaSteps { get; set; }

        public RankDelta()
        {
        }

        public RankDelta(Rank start, Rank end)
        {
            RankStart = start;
            RankEnd = end;
        }
    }
}
