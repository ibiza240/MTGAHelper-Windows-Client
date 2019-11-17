using System;
using System.Collections.Generic;
using MTGAHelper.Lib.Config.Users;

namespace MTGAHelper.Entity.OutputLogParsing
{
    public interface IDraftPack
    {
        List<string> draftPack { get; }
    }

    public class DraftMakePickRaw : IDraftPack
    {
        public string playerId { get; set; }
        public string eventName { get; set; }
        public string draftId { get; set; }
        public string draftStatus { get; set; }
        public int packNumber { get; set; }
        public int pickNumber { get; set; }
        public List<string> draftPack { get; set; }
        public List<string> pickedCards { get; set; }
        public double requestUnits { get; set; }
    }
}
