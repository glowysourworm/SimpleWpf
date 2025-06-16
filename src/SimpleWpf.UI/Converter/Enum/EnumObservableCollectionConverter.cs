using System.Globalization;
using System.Windows.Data;

using SimpleWpf.UI.Collection;

namespace SimpleWpf.UI.Converter
{
    /// <summary>
    /// Converter for the EnumObservableCollection. The parameter must carry the enum type.
    /// </summary>
    public class EnumObservableCollectionConverter : IValueConverter
    {
        /// <summary>
        /// Converts the enum value to an EnumObservableCollection
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            try
            {
                var agnosticCollection = typeof(EnumObservableCollection<>);
                var enumType = value.GetType();
                var resultType = agnosticCollection.MakeGenericType(enumType);
                var collection = Activator.CreateInstance(resultType);

                // Call method to set the EnumValue
                var propertyInfo = resultType.GetProperty("EnumValue");

                // Set property EnumValue with the bound converter value
                propertyInfo.SetValue(collection, value);

                return collection;
            }
            catch (Exception ex)
            {
                throw new Exception("EnumObservableCollectionConverter must bind to an Enum type. TwoWay binding is supported; and the result is an EnumObservableCollection");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            try
            {
                var enumValueProperty = value.GetType().GetProperty("EnumValue");
                return enumValueProperty.GetValue(value);
            }
            catch (Exception ex)
            {
                throw new Exception("EnumObservableCollectionConverter must bind to an Enum type. TwoWay binding is supported; and the result is an EnumObservableCollection");
            }
        }
    }
}
