using System.Globalization;
using System.Windows.Data;

namespace SimpleWpf.UI.Converter
{
    public class TimeSpanMillisecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            var timeSpan = (TimeSpan)value;

            return timeSpan.TotalMilliseconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            var milliseconds = (int)value;

            return TimeSpan.FromMilliseconds(milliseconds);
        }
    }
}
