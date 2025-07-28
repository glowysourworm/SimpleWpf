using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace SimpleWpf.UI.Converter
{
    /// <summary>
    /// Converts a list of generic type to an observable collection - using the parameter as the type
    /// </summary>
    public class ListToObservableCollectionParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Binding.DoNothing;

            try
            {
                var list = value as IList;

                var collectionType = typeof(ObservableCollection<>);
                var collectionGeneric = collectionType.MakeGenericType((Type)parameter);
                var collection = Activator.CreateInstance(collectionGeneric);
                var addMethod = collectionGeneric.GetMethod("Add");

                foreach (var item in list)
                {
                    addMethod.Invoke(collection, new object[] { item });
                }

                return collection;
            }
            catch (Exception ex)
            {
                throw new Exception("ListToObservableCollectionParameterConverter must be bound to an array (of some type), with the converter parameter specifying the type.");
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

                var resultType = typeof(List<>);
                var resultGeneric = resultType.MakeGenericType((Type)parameter);
                var addMethod = resultGeneric.GetMethod("Add");
                var resultCtor = resultGeneric.GetConstructor(new Type[] { });

                var result = resultCtor.Invoke(new object[] { });

                for (int index = 0; index < count; index++)
                {
                    // Get collection's current item
                    var item = indexerGetMethod.Invoke(value, new object[] { index });

                    // Add to the list
                    addMethod.Invoke(result, new object[] { item });
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("ListToObservableCollectionParameterConverter must be bound to an array (of some type), with the converter parameter specifying the type.");
            }
        }
    }
}
