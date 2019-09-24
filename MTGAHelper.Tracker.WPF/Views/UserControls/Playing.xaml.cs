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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Playing.xaml
    /// </summary>
    public partial class Playing : UserControl
    {
        public Playing()
        {
            InitializeComponent();
        }

        public void Init(MainWindowVM vm)
        {
            DataContext = vm;
            CardsLibrary.SetDataContext(vm.InMatchState.MyLibrary);
            CardsOpponent.SetDataContext(vm.InMatchState.OpponentCardsSeen);
            //FullDeck.SetDataContext(vm.InMatchState.FullDeck);
        }
    }
}
