using MTGAHelper.Tracker.DraftHelper.Shared.Config;

namespace MTGAHelper.Tracker.DraftHelper.Shared.Models
{
    public class InputModel
    {
        public string RatingsSource { get; set; }
        public string Set { get; set; }
        public string FilepathScreenshot { get; set; }
        //public ICollection<int> CardNamesYCoords { get; set; }
        //public ICollection<int> BordersXCoords { get; set; }
        //public float ZoomFactor { get; set; }
        //public float SimilarityThreshold { get; set; } = 0.9f;
        //public int TopPctRatingsToAnalyze { get; set; } = 100;
        public ConfigResolution ConfigResolution { get; set; }
        public int NbColumns { get; set; } = 5;
        public int NbRows { get; set; } = 3;
        public int NbCards { get; set; }
    }
}
