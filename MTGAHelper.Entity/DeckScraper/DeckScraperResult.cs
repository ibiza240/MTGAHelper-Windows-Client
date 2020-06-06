using System.Collections.Generic;
using MTGAHelper.Entity.Config.Decks;

namespace MTGAHelper.Entity.DeckScraper
{
    public class DeckScraperResult
    {
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public int NbTotal { get; private set; }
        public int NbSuccess { get; private set; }
        public int NbIgnored { get; private set; }

        public ICollection<ConfigModelDeck> Decks { get; set; } = new ConfigModelDeck[0];

        public DeckScraperResult()
        {
        }

        //public DeckScraperResult(string error)
        //{
        //    Errors = new List<string> { error };
        //}

        public DeckScraperResult(int nbTotal, int nbSuccess, int nbIgnored, ICollection<ConfigModelDeck> decks)
        {
            NbTotal = nbTotal;
            NbSuccess = nbSuccess;
            NbIgnored = nbIgnored;
            Decks = decks;
        }
    }
}
