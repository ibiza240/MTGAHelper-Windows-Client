using System.Drawing;

namespace MTGAHelper.Tracker.DraftHelper.Shared.Config
{
    public class ConfigResolution
    {
        public Size Resolution { get; set; }
        public bool IsPanoramic { get; set; }
        public ConfigResolutionTemplate Template { get; set; }
    }

    public class ConfigResolutionTemplate
    {
        public Size ArtSize { get; set; }
        public Point FirstCardArtLocation { get; set; }
        public Point LastCardArtLocation { get; set; }
        public int NbColumns { get; set; }
        public int NbRows { get; set; }

        public float StepX => (LastCardArtLocation.X - FirstCardArtLocation.X) / (float)(NbColumns - 1);
        public float StepY => (LastCardArtLocation.Y - FirstCardArtLocation.Y) / (float)(NbRows - 1);

    }
}
