using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SimpleWpf.UI.Converter.TreeViewUI
{
    public class TreeViewItemMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            var itemIndent = (int)(value ?? 0);

            return new Thickness(itemIndent, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
