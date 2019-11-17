using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public class PayEntryRaw
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
