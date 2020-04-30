using AutoMapper;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;
using System.Windows;
using System.Windows.Input;
using Point = System.Windows.Point;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class OptionsWindow
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public OptionsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Dependency Injection Initializer
        /// </summary>
        /// <param name="configApp"></param>
        /// <param name="ratingSources"></param>
        /// <returns></returns>
        public OptionsWindow Init(ConfigModel configApp, string[] ratingSources)
        {
            var vm = Mapper.Map<OptionsWindowVM>(configApp);
            vm.ShowLimitedRatingsSources = ratingSources;
            DataContext = vm;
            return this;
        }

        /// <summary>
        /// Handle key presses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }

        /// <summary>
        /// Close the window on menu icon click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Drag Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (WindowState == WindowState.Maximized)
            {
                Point mousePos = PointToScreen(Mouse.GetPosition(this));
                Top = 0;
                Left = mousePos.X - 20;
                WindowState = WindowState.Normal;
            }

            // Begin dragging the window
            DragMove();
        }
    }
}
