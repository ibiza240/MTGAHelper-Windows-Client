using System.Collections.Generic;
using MTGAHelper.Entity.OutputLogParsing;

namespace MTGAHelper.Lib.OutputLogParser.Models.UnityCrossThreadLogger
{
    public class CompleteDraftRaw : ICardPool
    {
        public string Id { get; set; }
        public string InternalEventName { get; set; }
        public object PlayerId { get; set; }
        public ModuleInstanceData ModuleInstanceData { get; set; }
        public string CurrentEventState { get; set; }
        public string CurrentModule { get; set; }
        public List<int> CardPool { get; set; }
        public object CourseDeck { get; set; }
        public List<object> PreviousOpponents { get; set; }
    }
}
