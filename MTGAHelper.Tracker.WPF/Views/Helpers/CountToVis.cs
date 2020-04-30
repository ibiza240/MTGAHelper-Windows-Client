using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MTGAHelper.Tracker.WPF.Views.Helpers
{
    /// <summary>
    /// Converts a number with parameter representing maximum to visibility
    /// </summary>
    public class MaxCountToVisibilityConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty MaxCountProperty =
            DependencyProperty.Register(nameof(MaxCount), typeof(int), typeof(MaxCountToVisibilityConverter),
                new PropertyMetadata(0));

        private int MaxCount
        {
            get => (int)GetValue(MaxCountProperty);
            set => SetValue(MaxCountProperty, value);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the value is null, always return collapsed
            if (value == null)
                return Visibility.Collapsed;

            // Set local count
            int max = MaxCount;

            // Try to convert the parameter, which overrides the dependency property
            if (parameter != null && int.TryParse(parameter.ToString(), out int m))
                max = m;

            // Attempt to convert the input value
            return int.TryParse(value.ToString(), out int input)
                ? input <= max
                    ? Visibility.Visible
                    : Visibility.Collapsed
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
