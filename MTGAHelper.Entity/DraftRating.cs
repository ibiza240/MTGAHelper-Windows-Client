using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    //public enum DraftRatingSourceEnum
    //{
    //    ChannelFireballLSV,
    //    DraftSim,
    //}

    public class DraftRatingTopCard
    {
        public int Rank { get; set; }
        public string Name { get; set; }

        public DraftRatingTopCard(int rank, string name)
        {
            Rank = rank;
            Name = name;
        }
    }

    public class DraftRating
    {
        //public Card Card { get; set; }
        public string CardName { get; set; }
        public string Rating { get; set; }
        public string Description { get; set; }
    }

    public class DraftRatingScraperResultForSet
    {
        public ICollection<DraftRating> Ratings { get; set; } = new DraftRating[0];
        public Dictionary<string, ICollection<DraftRatingTopCard>> TopCommonCardsByColor { get; set; } = new Dictionary<string, ICollection<DraftRatingTopCard>>();
    }

    public class DraftRatings
    {
        public Dictionary<string, DraftRatingScraperResultForSet> RatingsBySet { get; set; } = new Dictionary<string, DraftRatingScraperResultForSet>();
        //public Dictionary<string, DraftRating> DictRatingByCardName { get; set; } = new Dictionary<string, DraftRating>();
    }
}
