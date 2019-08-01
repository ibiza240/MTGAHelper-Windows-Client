using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    public class DraftRatingTop5Card
    {
        public int Rank { get; set; }
        public string Name { get; set; }

        public DraftRatingTop5Card(int rank, string name)
        {
            Rank = rank;
            Name = name;
        }
    }

    public class DraftRating
    {
        public Card Card { get; set; }
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
        public Dictionary<string, ICollection<DraftRatingTop5Card>> Top5CardsByColor { get; set; } = new Dictionary<string, ICollection<DraftRatingTop5Card>>();
    }
}
