using System.Collections.Generic;

namespace MTGAHelper.Tracker.DraftHelper.Shared.Models
{
    public class OutputModel
    {
        public int CardId { get; set; }
        public string CardName { get; set; }
        public int LabelIndex { get; set; }
        public float Similarity { get; set; }
        public float RatingValue { get; set; }

        public string ToString(Dictionary<int, string> mappingIds)
        {
            return $"{RatingValue} {CardName} ({CardId}) at {LabelIndex} ({Similarity:P})";
        }
    }
}
