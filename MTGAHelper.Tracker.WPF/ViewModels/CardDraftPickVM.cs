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
        public CardVM CardVM { get; set; }
    }
}
