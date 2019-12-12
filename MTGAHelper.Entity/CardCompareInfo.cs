using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    public class CardCompareInfo
    {
        public int GrpId { get; set; }
        public int NbMissing { get; set; }
        public float MissingWeight { get; set; }
        public int NbDecksMain { get; set; }
        public int NbDecksSideboardOnly { get; set; }
    }
}
