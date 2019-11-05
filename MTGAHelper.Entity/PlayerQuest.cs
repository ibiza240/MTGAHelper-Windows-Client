using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    public class PlayerQuest
    {
        public string QuestTrack { get; set; }
        public string LocKey { get; set; }
        public int EndingProgress { get; set; }
        public int Goal { get; set; }
    }
}
