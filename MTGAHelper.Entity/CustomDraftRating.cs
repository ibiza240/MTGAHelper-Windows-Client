using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    public class CustomDraftRating
    {
        public string Set { get; set; }
        public string Name { get; set; }
        public int? Rating { get; set; }
        public string Note { get; set; }
    }
}
