using System;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;

namespace MTGAHelper.Tracker.WPF.Views.Helpers
{
    public class MissingCardsToColorScaleConverter : IValueConverter
    {
        public int Threshold { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case 4:
                    return Brushes.Green;
                case 3:
                    return Brushes.LightGreen;
                case 2:
                    return Brushes.Yellow;
                case 1:
                    return Brushes.Orange;
                case 0:
                    return Brushes.Red;
                default:
                    return Brushes.Red;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
