using System.Collections.Generic;

namespace MTGAHelper.Tracker.WPF.Models.OutputLog
{
    public class DraftMakePick
    {
        public string playerId { get; set; }
        public string eventName { get; set; }
        public string draftId { get; set; }
        public string draftStatus { get; set; }
        public int packNumber { get; set; }
        public int pickNumber { get; set; }
        public List<string> draftPack { get; set; }
        public List<string> pickedCards { get; set; }
        public double requestUnits { get; set; }
    }
}
