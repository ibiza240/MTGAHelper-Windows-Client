using System.Collections.Generic;

namespace MTGAHelper.Tracker.DraftHelper.Shared.Models
{
    public class DraftRatings
    {
        public Dictionary<string, DraftRatingScraperResultForSet> RatingsBySet { get; set; } = new Dictionary<string, DraftRatingScraperResultForSet>();
    }

    public class DraftRatingScraperResultForSet
    {
        public ICollection<DraftRating> Ratings { get; set; } = new DraftRating[0];
        //public Dictionary<string, ICollection<DraftRatingTopCard>> TopCommonCardsByColor { get; set; } = new Dictionary<string, ICollection<DraftRatingTopCard>>();
    }

}
