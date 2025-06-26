using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace SimpleWpf.UI.Converter
{
    public class BoolToWaitCursorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Binding.DoNothing;

            if ((bool)value)
                return Cursors.Wait;

            return (Cursor)parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
