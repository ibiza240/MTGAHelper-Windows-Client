namespace MTGAHelper.Tracker.WPF.Models
{
    public class DraftHelperOutputModel
    {
        public int CardId { get; set; }
        public string CardName { get; set; }
        public string Location { get; set; }
        public float Similarity { get; set; }
        public float RatingValue { get; set; }
    }
}
