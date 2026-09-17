using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace SimpleWpf.UI.Converter
{
    public class CollectionLastElementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Binding.DoNothing;

            var enumerable = value as IEnumerable;
            var property = parameter as string;

            if (enumerable == null || property == null)
                return Binding.DoNothing;

            object returnItem = null;

            foreach (var item in enumerable)
            {
                returnItem = item;
            }

            return returnItem?.GetType()?.GetProperty(property)?.GetValue(returnItem) ?? Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
