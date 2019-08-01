using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Tracker.WPF.Models
{
    public class Card
    {
        public int ArenaId { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
    }

    public class CardDraftPick : Card
    {
        public float Weight { get; set; }
    }
}
