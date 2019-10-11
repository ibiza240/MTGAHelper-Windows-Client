using AutoMapper;
using Microsoft.Extensions.Options;
using MTGAHelper.Tracker.WPF.Config;
using MTGAHelper.Tracker.WPF.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        public OptionsWindow Init(ConfigModelApp configApp)
        {
            var vm = Mapper.Map<OptionsWindowVM>(configApp);
            DataContext = vm;

            return this;
        }

        void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }
    }
}
