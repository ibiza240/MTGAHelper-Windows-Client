using System;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger.EventSetDeck
{
    public class EventSetDeckResult : RequestParamsResult2<EventSetDeckRaw>
    {
    }

    public partial class EventSetDeckRaw
    {
        public string EventName { get; set; }
        public CourseDeckSummary Summary { get; set; }
        public EventDeckRaw Deck { get; set; }
    }

    public partial class EventDeckRaw
    {
        public CourseDeckCard[] MainDeck { get; set; }
        public CourseDeckCard[] ReducedSideboard { get; set; }
        public CourseDeckCard[] Sideboard { get; set; }
        public object[] CommandZone { get; set; }
        public object[] Companions { get; set; }
        public CardSkin[] CardSkins { get; set; }
        public bool DoPreferReducedSideboard { get; set; }
    }

    public partial class CardSkin
    {
        public long GrpId { get; set; }
        public string Ccv { get; set; }
    }

    public partial class CourseDeckCard
    {
        public int CardId { get; set; }
        public int Quantity { get; set; }
    }

    public partial class CourseDeckSummary
    {
        public Guid DeckId { get; set; }
        public string Mana { get; set; }
        public string Name { get; set; }
        public Attribute[] Attributes { get; set; }
        public string Description { get; set; }
        public int? DeckTileId { get; set; }
        public bool IsCompanionValid { get; set; }
        public string CardBack { get; set; }
    }

    public partial class Attribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}