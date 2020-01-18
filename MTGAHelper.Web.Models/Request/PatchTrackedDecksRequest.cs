using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Request
{
    public class PatchTrackedDecksRequest
    {
        public bool? DoTrack { get; set; }
        public ICollection<string> Decks { get; set; }
    }
}
