using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows.Data;

using SimpleWpf.Extensions;
using SimpleWpf.UI.ViewModel.EnumUI;

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
                var collection = new ObservableCollection<EnumItemViewModel>();
                var enumType = (Type)value;

                // Initialize Collection
                foreach (Enum enumValue in Enum.GetValues(enumType))
                {
                    var item = new EnumItemViewModel();
                    var displayAttribute = enumValue.GetAttribute<DisplayAttribute>();

                    item.Value = enumValue;
                    item.Name = Enum.GetName(enumType, enumValue) ?? string.Empty;
                    item.DisplayName = displayAttribute?.Name ?? item.Name ?? string.Empty;
                    item.Description = displayAttribute?.Description ?? string.Empty;

                    collection.Add(item);
                }

                return collection;
            }
            catch (Exception ex)
            {
                throw new Exception("EnumObservableCollectionConverter must bind to an Enum type. TwoWay binding is not supported; and the result is an ObservableCollection");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("EnumObservableCollectionConverter must bind to an Enum type. TwoWay binding is not supported; and the result is an ObservableCollection");
        }
    }
}
