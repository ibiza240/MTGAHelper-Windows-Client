using System.Collections.Generic;

namespace MTGAHelper.Web.UI.Model.Response.Dto
{
    public class DeckDto
    {
        public string OriginalDeckId { get; set; }
        public string Id { get; set; }
        public uint Hash { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ScraperTypeId { get; set; }
        public ICollection<DeckCardDto> CardsMain { get; set; }
        public ICollection<DeckCardDto> CardsMainOther { get; set; }
        public ICollection<DeckCardDto> CardsSideboard { get; set; }
        public string MtgaImportFormat { get; set; }

        public ICollection<DeckCompareResultDto> CompareResults { get; set; }

        public ICollection<DeckManaCurveDto> ManaCurve { get; set; }
    }

    public class DeckCompareResultDto
    {
        public string Set { get; set; }
        public int NbMissing { get; set; }
        public float MissingWeightTotal { get; set; }
    }


    public class DeckManaCurveDto
    {
        public int ManaCost { get; set; }
        public int NbCards { get; set; }
        public int PctHeight { get; set; }
    }
}
