using System;
using System.Collections.Generic;
using System.Text;

namespace MTGAHelper.Tracker.WPF.ViewModels
{
    public class CardPopupVM
    {
        public ObservableProperty<string> ImageCardUrl { get; set; } = new ObservableProperty<string>(null);
    }
}
