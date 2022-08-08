using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.GetDeck
{
    public class MainDeck
    {
        public int cardId { get; set; }
        public int quantity { get; set; }
    }

    public class CardSkin
    {
        public int GrpId { get; set; }
        public string CCV { get; set; }
    }

    public partial class GetDeckRaw
    {
        public List<MainDeck> MainDeck { get; set; }
        public List<MainDeck> ReducedSideboard { get; set; }
        public List<MainDeck> Sideboard { get; set; }
        public List<MainDeck> CommandZone { get; set; }
        public List<MainDeck> Companions { get; set; }
        public List<CardSkin> CardSkins { get; set; }
        public bool DoPreferReducedSideboard { get; set; }
    }

    public class GetDeckResult : MtgaOutputLogPartResultBase<GetDeckRaw>
    {
    }
}