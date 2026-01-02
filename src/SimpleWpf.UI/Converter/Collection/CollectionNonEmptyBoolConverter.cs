using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SimpleWpf.UI.Converter
{
    /// <summary>
    /// Returns true / false when collection is non-empty / empty
    /// </summary>
    public class CollectionNonEmptyBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as ICollection;
            if (collection == null)
                return Visibility.Collapsed;

            return collection.Count > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
