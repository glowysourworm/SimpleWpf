using System.Globalization;
using System.Windows.Data;

namespace SimpleWpf.UI.Converter
{
    /// <summary>
    /// Provides the parameter if the boolean value is false. Otherwise, returns Binding.DoNothing.
    /// </summary>
    public class ParameterProviderBoolInverseConverter : IValueConverter
    {
        private object _originalParameter;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Binding.DoNothing;

            _originalParameter = parameter;

            if (!(bool)value)
                return parameter;

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _originalParameter;
        }
    }
}
