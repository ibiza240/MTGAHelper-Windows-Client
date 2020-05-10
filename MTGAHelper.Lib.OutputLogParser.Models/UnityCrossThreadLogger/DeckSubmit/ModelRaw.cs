using MTGAHelper.Entity.OutputLogParsing;
using System.Collections.Generic;

namespace MTGAHelper.Lib.IO.Reader.MtgaOutputLog.UnityCrossThreadLogger
{
    //public class ModuleInstanceData
    //{
    //    public bool DeckSelected { get; set; }
    //}

    public class DeckSubmitRaw : ICardPool
    {
        public string Id { get; set; }
        public string publicEventName { get; set; }
        public object PlayerId { get; set; }
        public ModuleInstanceData ModuleInstanceData { get; set; }
        public string CurrentEventState { get; set; }
        public string CurrentModule { get; set; }
        public List<int> CardPool { get; set; }
        public CourseDeckRaw CourseDeck { get; set; }
        public List<object> PreviousOpponents { get; set; }
    }
}
