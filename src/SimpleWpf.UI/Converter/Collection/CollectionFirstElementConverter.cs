using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace SimpleWpf.UI.Converter
{
    public class CollectionFirstElementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Binding.DoNothing;

            var enumerable = value as IEnumerable;
            var property = parameter as string;

            if (enumerable == null || property == null)
                return Binding.DoNothing;

            foreach (var item in enumerable)
            {
                if (item != null)
                    return item.GetType().GetProperty(property)?.GetValue(item) ?? Binding.DoNothing;

                else
                    return Binding.DoNothing;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
