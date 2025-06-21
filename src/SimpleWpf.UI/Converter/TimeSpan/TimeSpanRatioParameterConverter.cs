using System.Globalization;
using System.Windows.Data;

namespace SimpleWpf.UI.Converter
{
    public class TimeSpanRatioParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Binding.DoNothing;

            var timeSpan = (TimeSpan)value;
            var totalTime = (TimeSpan)parameter;

            return timeSpan.TotalMilliseconds / (float)totalTime.TotalMilliseconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Binding.DoNothing;

            var ratio = (float)value;
            var totalTime = (TimeSpan)parameter;

            return TimeSpan.FromMilliseconds(totalTime.TotalMilliseconds * ratio);
        }
    }
}
