using MTGAHelper.Tracker.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class CardDraftPickVM : CardDraftPick
    {
        //public bool ShowMtgaHelperSays => Weight > 0 || RareDraftPickEnum != Entity.RaredraftPickReasonEnum.None;
        public string NbDecksUsedInfo => $"Played in {NbDecksUsedMain} tracked deck{(NbDecksUsedMain == 1 ? "" : "s")} and {NbDecksUsedSideboard} sideboard{(NbDecksUsedSideboard == 1 ? "" : "s")}";

        public CardVM CardVM { get; set; }
    }
}
