using System.Collections.Generic;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public interface IDraftPack
    {
        List<string> DraftPack { get; }
    }

    public class DraftPickStatusRaw : IDraftPack
    {
        public string Result { get; set; }
        public string EventName { get; set; }
        public string DraftStatus { get; set; }
        public int PackNumber { get; set; }
        public int PickNumber { get; set; }
        public List<string> DraftPack { get; set; }
        public List<string> PickedCards { get; set; }
    }
}