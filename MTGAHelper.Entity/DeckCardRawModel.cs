using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    public class DeckCardRawModel
    {
        public int Qty { get; set; }
        public string Code { get; set; }
        public bool IsSideboard { get; set; }
        public bool IsMainOther { get; set; }
    }
}
