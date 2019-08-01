using MTGAHelper.Entity;
using MTGAHelper.Lib.CollectionDecksCompare;
using MTGAHelper.Lib.Config;

namespace MTGAHelper.Lib.Model
{
    public class DeckDetails
    {
        public string OriginalDeckId { get; set; }
        public ConfigModelDeck Config { get; set; }
        public IDeck Deck { get; set; }
        public string MtgaImportFormat { get; set; }
    }

    public class DeckTrackedDetails : DeckDetails
    {
        public CardRequiredInfoByDeck CardsRequired { get; set; }
        public CardsMissingResult CompareResult { get; set; }
    }
}
