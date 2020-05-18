using System.Collections.Generic;

namespace MTGAHelper.Entity
{
    public class DraftPickProgress
    {
        public DraftPickProgress()
        {
        }

        public DraftPickProgress(IList<int> cardPool)
        {
            DraftPack = cardPool;
        }

        public string EventName { get; set; }
        public string DraftId { get; set; }
        public int PackNumber { get; set; }     // 0 based
        public int PickNumber { get; set; }     // 0 based
        public IList<int> DraftPack { get; set; } = new List<int>();
        public IList<int> PickedCards { get; set; } = new List<int>();
    }
}
