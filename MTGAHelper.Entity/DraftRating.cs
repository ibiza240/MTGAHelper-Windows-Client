using System;
using System.Collections.Generic;
using System.Globalization;

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
        public string CardName { get; set; }
        public string RatingToDisplay { get; set; }

        [Obsolete]
        public string Rating { get; set; }

        public float RatingValue { get; set; }
        public string Description { get; set; }

        public static float GetRatingScale(string ratingSource)
        {
            var rating = ratingSource switch
            {
                "Mtg Community Review" => 12f,
                "Infinite Mythic Edition" => 5.4f,
                "MTG Arena Zone" => 5.4f,
                "Your custom ratings" => 10f,
                _ => 5f,
            };
            return rating;
        }
    }

    public class DraftRatingScraperResultForSet
    {
        public ICollection<DraftRating> Ratings { get; set; } = new DraftRating[0];
        public Dictionary<string, ICollection<DraftRatingTopCard>> TopCommonCardsByColor { get; set; } = new Dictionary<string, ICollection<DraftRatingTopCard>>();
    }

    public class DraftRatings
    {
        public Dictionary<string, DraftRatingScraperResultForSet> RatingsBySet { get; set; } = new Dictionary<string, DraftRatingScraperResultForSet>();
    }
}
