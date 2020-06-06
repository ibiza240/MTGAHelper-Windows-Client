using System.Collections.Generic;
using System.Linq;
using MTGAHelper.Entity.DeckScraper;

namespace MTGAHelper.Web.UI.Model.Response
{
    public class DecksDownloadedResponse
    {
        public ICollection<string> DecksIds { get; set; }
        public int NbTotal { get; set; }
        public int NbSuccess { get; set; }
        public int NbIgnored { get; set; }
        public ICollection<string> Errors { get; set; }
        public ICollection<string> Warnings { get; set; }

        public DecksDownloadedResponse(DeckScraperResult result)
        {
            DecksIds = result.Decks.Select(i => i.Id).ToArray();
            NbTotal = result.NbTotal;
            NbSuccess = result.NbSuccess;
            NbIgnored = result.NbIgnored;
            Errors = result.Errors;
            Warnings = result.Warnings;
        }
    }
}
