using AutoMapper;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace MTGAHelper.Tracker.WPF.Views
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        //public OptionsWindow()
        //{
        //    // For WPF
        //    vm = new OptionsWindowVM();
        //}

        public OptionsWindow()
        {
            InitializeComponent();
        }

        public OptionsWindow Init(ConfigModelApp configApp, string[] ratingSources)
        {
            var vm = Mapper.Map<OptionsWindowVM>(configApp);
            vm.ShowLimitedRatingsSources = ratingSources;
            DataContext = vm;

            return this;
        }

        void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }
    }
}
