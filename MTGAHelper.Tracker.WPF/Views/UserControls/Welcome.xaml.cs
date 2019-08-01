using Microsoft.Extensions.Options;
using MTGAHelper.Tracker.WPF.Config;
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

namespace MTGAHelper.Tracker.WPF.Views.UserControls
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : UserControl
    {
        MainWindow mainWindow => (MainWindow)Window.GetWindow(this);

        public Welcome()
        {
            InitializeComponent();
        }

         void HyperlinkOptions_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ShowDialogOptions();
        }
    }
}
