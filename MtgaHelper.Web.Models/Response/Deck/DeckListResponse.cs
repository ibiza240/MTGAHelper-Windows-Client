using MTGAHelper.Entity;
using MTGAHelper.Lib;
using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Response
{
    public class DeckListResponse
    {
        public int TotalDecks { get; set; }
        public ICollection<DeckSummary> Decks { get; set; }

        public DeckListResponse(int totalDecks, ICollection<DeckSummary> decks)
        {
            var util = new Util();

            TotalDecks = totalDecks;
            Decks = decks;

            foreach (var d in Decks)
                d.Hash = util.To32BitFnv1aHash(d.Id);
        }
    }
}
