using System;
using System.Windows.Data;

namespace MTGAHelper.Tracker.WPF.Views.Helpers
{
    public class BoolToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? FalseValue : System.Convert.ToBoolean(value) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value?.Equals(TrueValue) ?? false;
        }
    }

    public class BoolToStringConverter : BoolToValueConverter<string> { }
    public class BoolToIntegerConverter : BoolToValueConverter<int> { }

}
