namespace MTGAHelper.Tracker.DraftHelper.Shared.Models
{
    public class Card
    {
        public int GrpId { get; set; }
        public string Name { get; set; }
        public string Set => SetScryfall;
        public string SetScryfall { get; set; }
        public string Rarity { get; set; }
    }
}
