using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SimpleWpf.UI.Converter
{
    public class BoolInverseVisibilityCollapseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ReferenceEquals(value, null) ||
                value == DependencyProperty.UnsetValue)
                return Binding.DoNothing;

            if ((bool)value)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
