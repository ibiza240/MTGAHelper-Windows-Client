using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace MTGAHelper.Tracker.WPF.Views.Helpers
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public int Threshold { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value > Threshold ? "Visible" : "Collapsed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
