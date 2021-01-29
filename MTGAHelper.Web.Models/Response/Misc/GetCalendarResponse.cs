using System;
using MTGAHelper.Entity.Config.Decks;

namespace MTGAHelper.Lib.Config
{
    public class GetCalendarResponse
    {
        public string Title { get; set; }
        public string DateRange { get; set; }

        //public string Description { get; set; }
        public string Image { get; set; }
    }
}
