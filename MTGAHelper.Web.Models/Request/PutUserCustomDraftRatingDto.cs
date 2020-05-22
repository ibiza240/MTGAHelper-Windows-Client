using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Web.Models.Request
{
    public class PutUserCustomDraftRatingDto
    {
        public int IdArena { get; set; }
        public int? Rating { get; set; }
        public string Note { get; set; }
    }
}
