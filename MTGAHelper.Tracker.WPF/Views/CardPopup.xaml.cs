using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for CardPopup.xaml
    /// </summary>
    public partial class CardPopup : Window
    {
        CardPopupVM vm = new CardPopupVM();

        public CardPopup()
        {
            DataContext = vm;
            InitializeComponent();
        }

        internal void ShowCard(string imageCardUrl)
        {
            vm.ImageCardUrl.Value = imageCardUrl;
            this.Show();
        }
    }
}
