using MTGAHelper.Entity;
using System;
using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Response
{
    public class DeckSummaryResponseDto
    {
        public string Id { get; set; }
        public uint Hash { get; set; }
        public string Name { get; set; }
        public string ScraperTypeId { get; set; }
        public string Color { get; set; }
        public string Url { get; set; }
        public DateTime? DateCreated { get; set; }
    }

    public class DeckListResponse<T> where T : DeckSummaryResponseDto
    {
        public int TotalDecks { get; set; }
        public ICollection<T> Decks { get; set; }

        public DeckListResponse()
        {
        }

        public DeckListResponse(int totalDecks, ICollection<T> decks)
        {
            var util = new Util();

            TotalDecks = totalDecks;
            Decks = decks;

            foreach (var d in Decks)
                d.Hash = util.To32BitFnv1aHash(d.Id);
        }
    }
}
