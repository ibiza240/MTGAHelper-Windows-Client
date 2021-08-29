using System.Collections.Generic;
using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger
{
    public class CompleteDraftResult : MtgaOutputLogPartResultBase<CompleteDraftRaw>, ICardPool
    {
        public List<int> CardPool
        {
            get
            {
                return Raw.CardPool;
            }
            set
            {
                Raw.CardPool = value;
            }
        }
    }
}