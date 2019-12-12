using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MTGAHelper.Tracker.WPF.Views.Helpers
{
    public class RatingFloatToColorScaleConverter : IValueConverter
    {
        public byte MinGray { get; set; } = 0;
        public byte MaxGray { get; set; } = 255;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pct = (float)value / 5f;
            var valueGray = (byte)Math.Max(50, MinGray + (int)(pct * (MaxGray - MinGray)));

            return new SolidColorBrush(Color.FromRgb(valueGray, valueGray, valueGray));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
