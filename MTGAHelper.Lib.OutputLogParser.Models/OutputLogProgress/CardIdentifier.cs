using System.Collections.Generic;

namespace MTGAHelper.Lib.OutputLogProgress
{
    public class CardIdentifier
    {
        public CardIdentifier(int i)
        {
            InitialId = i;
            Ids = new List<int> { i };
        }

        public int InitialId { get; set; }
        public int GrpId { get; set; }

        public IList<int> Ids { get; set; }
    }
}
