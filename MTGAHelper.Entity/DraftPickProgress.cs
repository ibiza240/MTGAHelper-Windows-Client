using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    public class DraftPickProgress
    {
        public DraftPickProgress()
        {
        }

        public DraftPickProgress(ICollection<int> cardPool)
        {
            DraftPack = cardPool;
        }

        public string EventName { get; set; }
        public string DraftId { get; set; }
        public int PackNumber { get; set; }
        public int PickNumber { get; set; }
        public ICollection<int> DraftPack { get; set; }
        public ICollection<int> PickedCards { get; set; }
    }
}
