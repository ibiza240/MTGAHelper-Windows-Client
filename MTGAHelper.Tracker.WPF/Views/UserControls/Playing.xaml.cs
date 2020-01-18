using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Playing.xaml
    /// </summary>
    public partial class Playing : UserControl
    {
        MainWindowVM vmParent;
        CardListWindow windowOpponentCardsSeen;

        MainWindow mainWindow => (MainWindow)Window.GetWindow(this);

        public Playing()
        {
            InitializeComponent();
        }

        public void Init(MainWindowVM vmParent, WindowSettings windowSettingsOpponentCards)
        {
            this.vmParent = vmParent;
            windowOpponentCardsSeen = new CardListWindow("Opponent cards seen", vmParent.InMatchState.OpponentCardsSeen, mainWindow);

            if ((windowSettingsOpponentCards?.Position?.X ?? 0) != 0 ||
                (windowSettingsOpponentCards?.Position?.Y ?? 0) != 0 ||
                (windowSettingsOpponentCards?.Size?.X ?? 0) != 0 //||
                //(windowSettingsOpponentCards?.Size?.Y ?? 0) != 0
            )
            {
                windowOpponentCardsSeen.Width = (double)windowSettingsOpponentCards?.Size?.X;
                //windowOpponentCardsSeen.Height = (double)windowSettingsOpponentCards?.Size?.Y;

                windowOpponentCardsSeen.WindowStartupLocation = WindowStartupLocation.Manual;
                windowOpponentCardsSeen.Left = (double)windowSettingsOpponentCards?.Position?.X;
                windowOpponentCardsSeen.Top = (double)windowSettingsOpponentCards?.Position?.Y;
            }
        }

        //public void Init(MainWindowVM vm)
        //{
        //    DataContext = vm;
        //    //CardsMyLibrary.SetDataContext(vm.InMatchState.MyLibrary);
        //    //CardsMySideboard.SetDataContext(vm.InMatchState.MySideboard);
        //    //CardsOpponent.SetDataContext(vm.InMatchState.OpponentCardsSeen);
        //    //FullDeck.SetDataContext(vm.InMatchState.FullDeck);
        //}

        internal void SetCardPopupPosition(ForceCardPopupSideEnum side, int mainWindowTop, int mainWindowLeft, int mainWindowWidth)
        {
            CardsMyLibrary.SetCardPopupPosition(side, mainWindowTop, mainWindowLeft, mainWindowWidth);
            CardsMySideboard.SetCardPopupPosition(side, mainWindowTop, mainWindowLeft, mainWindowWidth);
            //CardsOpponent.SetCardPopupPosition(side, mainWindowTop, mainWindowLeft, mainWindowWidth);
            windowOpponentCardsSeen.SetCardsPopupPosition(side);
        }

        private void TabControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is TabItem tab && tab.Header.ToString() == "Opponent cards") //do not handle clicks on TabItem content but on TabItem itself
            {
                vmParent.InMatchState.ShowWindowOpponentCardsSeen = !vmParent.InMatchState.ShowWindowOpponentCardsSeen;
                windowOpponentCardsSeen.Left = mainWindow.Left - 40;
                windowOpponentCardsSeen.Top = mainWindow.Top + 230;
                windowOpponentCardsSeen.Visibility = Visibility.Visible;
                windowOpponentCardsSeen.Activate();
                e.Handled = true;
            }
        }

        internal void ShowHideOpponentCards(bool isVisible)
        {
            Dispatcher.Invoke(() =>
            {
                windowOpponentCardsSeen.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            });
        }

        internal void SetAlwaysOnTop(bool alwaysOnTop)
        {
            windowOpponentCardsSeen.Topmost = alwaysOnTop;
        }
    }
}
