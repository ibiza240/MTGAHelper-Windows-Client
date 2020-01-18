using System;
using System.Windows.Data;

namespace MTGAHelper.Tracker.WPF.Views.Helpers
{
    public class StringToValueConverter : IValueConverter
    {
        public string Value { get; set; }
        public string WhenEqual { get; set; }
        public string WhenNotEqual { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return WhenNotEqual;
            else
                return value.ToString() == Value ? WhenEqual : WhenNotEqual;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
