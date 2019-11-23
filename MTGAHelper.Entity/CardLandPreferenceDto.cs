using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Entity
{
    public class CardLandPreferenceDto
    {
        public string Name { get; set; }
        public string Set { get; set; }
        public string ImageCardUrl { get; set; }
        public int GrpId { get; set; }
        public bool IsSelected { get; set; }
    }

}
