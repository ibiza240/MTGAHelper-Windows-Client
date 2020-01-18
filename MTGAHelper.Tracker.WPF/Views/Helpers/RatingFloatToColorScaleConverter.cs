using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MTGAHelper.Tracker.WPF.Views.Helpers
{
    public class RatingFloatToColorScaleConverter : IMultiValueConverter
    {
        public byte MinGray { get; set; } = 0;
        public byte MaxGray { get; set; } = 255;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var value = values[0];
            var scale = values[1];
            var pct = (float)value / (float)scale;//5f;
            var valueGray = (byte)Math.Max(50, MinGray + (int)(pct * (MaxGray - MinGray)));

            return new SolidColorBrush(Color.FromRgb(valueGray, valueGray, valueGray));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
