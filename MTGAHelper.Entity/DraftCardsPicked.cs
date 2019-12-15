using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    public class DraftCardsPicked
    {
        public string Set { get; set; }
        public DateTime DatePicked { get; set; }
        public ICollection<Card> CardsPicked { get; set; }
    }

}
