using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    public class PlayerProgress
    {
        public string TrackName { get; set; }
        public int CurrentLevel { get; set; }
        public int CurrentExp { get; set; }
        public int CurrentOrbCount { get; set; }
    }
}
