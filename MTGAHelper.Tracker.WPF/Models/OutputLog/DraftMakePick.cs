using System.Collections.Generic;

namespace MTGAHelper.Tracker.WPF.Models.OutputLog
{
    public class DraftMakePick
    {
        public string PlayerId { get; set; }
        public string EventName { get; set; }
        public string DraftId { get; set; }
        public string DraftStatus { get; set; }
        public int PackNumber { get; set; }
        public int PickNumber { get; set; }
        public List<string> DraftPack { get; set; }
        public List<string> PickedCards { get; set; }
        public double RequestUnits { get; set; }
    }
}
