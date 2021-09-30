using System.Collections.Generic;

namespace MTGAHelper.Web.Models.Request
{
    public class PatchTrackedDecksRequest
    {
        public bool? DoTrack { get; set; }
        public ICollection<string> Decks { get; set; }
    }
}