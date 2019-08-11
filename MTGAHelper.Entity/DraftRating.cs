using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
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

    /// <summary>
    /// Dictionary is by Set
    /// </summary>
    public class DraftRatings : Dictionary<string, DraftRatingScraperResultForSet>
    {
        public Dictionary<Card, DraftRating> DictRatingByCard { get; set; } = new Dictionary<Card, DraftRating>();
    }

    public class DraftRatingScraperResultForSet
    {
        public ICollection<DraftRating> Ratings { get; set; } = new DraftRating[0];
        public Dictionary<string, ICollection<DraftRatingTopCard>> TopCommonCardsByColor { get; set; } = new Dictionary<string, ICollection<DraftRatingTopCard>>();
    }
}
