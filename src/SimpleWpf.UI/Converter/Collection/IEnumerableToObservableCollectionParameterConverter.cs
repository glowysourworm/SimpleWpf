using System.Collections.ObjectModel;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace SimpleWpf.UI.Converter
{
    /// <summary>
    /// Converts an array of generic type to an observable collection - using the parameter as the type
    /// </summary>
    public class IEnumerableToObservableCollectionParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Binding.DoNothing;

            try
            {
                var enumerable = value as IEnumerable;

                var collectionType = typeof(ObservableCollection<>);
                var collectionGeneric = collectionType.MakeGenericType((Type)parameter);
                var collection = Activator.CreateInstance(collectionGeneric);
                var addMethod = collectionGeneric.GetMethod("Add");

                foreach (var item in enumerable)
                {
                    addMethod.Invoke(collection, new object[] { item });
                }

                return collection;
            }
            catch (Exception ex)
            {
                throw new Exception("ArrayToObservableCollectionParameterConverter must be bound to an array (of some type), with the converter parameter specifying the type.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Binding.DoNothing;

            try
            {
                var collectionType = typeof(ObservableCollection<>);
                var collectionGeneric = collectionType.MakeGenericType((Type)parameter);                
                var countProperty = collectionGeneric.GetProperty("Count");
                var indexerGetMethod = collectionGeneric.GetProperties().First(x => x.GetIndexParameters().Length > 0).GetGetMethod();
                var count = (int)countProperty.GetValue(value);

                // NEED ENUMBERABLE COLLECTION TYPE
                var result = Array.CreateInstance((Type)parameter, count);

                for (int index = 0; index < count; index++)
                {
                    // Get collection's current item
                    var item = indexerGetMethod.Invoke(value, new object[] { index });

                    // Set to the array
                    result.SetValue(item, index);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("ArrayToObservableCollectionParameterConverter must be bound to an array (of some type), with the converter parameter specifying the type.");
            }
        }
    }
}
