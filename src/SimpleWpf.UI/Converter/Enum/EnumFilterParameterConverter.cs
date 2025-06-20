using System.Globalization;
using System.Windows.Data;

using SimpleWpf.Extensions;

namespace SimpleWpf.UI.Converter
{
    /// <summary>
    /// Returns true / false based on whether the enum passes the filter parameter test. (for flags / non-flags)
    /// </summary>
    public class EnumFilterParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Binding.DoNothing;

            if (!value.GetType().IsEnum)
                throw new ArgumentException("EnumFilterParameterConverter must be bound to an Enum type");

            if (!Enum.IsDefined(value.GetType(), value))
                throw new ArgumentException("Value of Enum is not defined:  EnumFilterParameterConverter.cs");

            if (!parameter.GetType().IsEnum)
                throw new ArgumentException("EnumFilterParameterConverter parameter must be an Enum type");

            if (!Enum.IsDefined(parameter.GetType(), value))
                throw new ArgumentException("Value of Enum (parameter) is not defined:  EnumFilterParameterConverter.cs");

            var enumFilter = (Enum)parameter;
            var enumValue = (Enum)value;

            // Flags
            if (enumValue.GetAttribute<FlagsAttribute>() != null)
            {
                return !enumValue.Has(enumFilter);
            }

            // Standard (non-flags)
            else
            {
                return enumValue != enumFilter;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
