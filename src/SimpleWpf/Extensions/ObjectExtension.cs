using System.Linq.Expressions;
using System.Reflection;

namespace SimpleWpf.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// Returns the First or Default attribute of the supplied type for the supplied object
        /// </summary>
        public static T GetAttribute<T>(this object value) where T : System.Attribute
        {
            var attributes = value.GetType().GetCustomAttributes(typeof(T), true);

            return attributes.Any() ? (T)attributes.First() : default(T);
        }

        public static bool ImplementsInterface<T>(this object value)
        {
            return value.GetType()
                        .GetInterfaces()
                        .Any(type => type.Equals(typeof(T)));
        }

        public static string[] GetPropertyPath(this object theObject, string propertyPath)
        {
            if (theObject == null)
                throw new NullReferenceException("Argument set to null!");

            if (string.IsNullOrWhiteSpace(propertyPath))
                throw new ArgumentException("Invalid use of property path ObjectExtension.GetPropertyPath");

            // Property selection for generics behaves better if you work with it in pieces
            //
            var propertyPathPieces = propertyPath.Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (propertyPathPieces.Length < 1)
                throw new ArgumentException("Invalid use of property path ObjectExtension.GetPropertyPath");

            return propertyPathPieces;
        }

        public static PropertyInfo GetPropertyInfo<T, V>(this T theObject, Expression<Func<T, V>> propertySelector)
        {
            var unaryExpression = propertySelector.Body as UnaryExpression;

            if (unaryExpression == null)
                throw new Exception("Invalid use of property selector ObjectExtension.GetPropertyInfo<T, V>");

            var memberInfo = unaryExpression.Operand as MemberExpression;

            if (memberInfo == null)
                throw new Exception("Invalid use of property selector ObjectExtension.GetPropertyInfo<T, V>");

            var propertyInfo = memberInfo.Member as PropertyInfo;

            if (propertyInfo == null)
                throw new Exception("Invalid use of property selector ObjectExtension.GetPropertyInfo<T, V>");

            return propertyInfo;
        }

        /// <summary>
        /// Returns property info for nested object instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="theObject"></param>
        /// <param name="propertyPath"></param>
        /// <param name="nextObject"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static PropertyInfo GetPropertyInfo<T>(this T theObject, string propertyPath, out object? nextObject)
        {
            // -> Validation
            var propertyPathPieces = GetPropertyPath(theObject, propertyPath);

            // Start Object
            nextObject = theObject;
            PropertyInfo? nextPropertyInfo = null;

            for (int index = 0; index < propertyPathPieces.Length; index++)
            {
                if (nextObject == null)
                    throw new NullReferenceException("Property reflection failure due to null reference in property recovery");

                nextPropertyInfo = nextObject.GetType().GetProperty(propertyPathPieces[index]);

                if (nextPropertyInfo == null)
                    throw new NullReferenceException("Property reflection failure due to improper path");

                // -> Next Property (object)
                if (index < propertyPathPieces.Length - 1)
                    nextObject = nextPropertyInfo.GetValue(nextObject);
            }

            return nextPropertyInfo;
        }

        public static object GetProperty<T>(this T theObject, string[] propertyPathPieces)
        {
            PropertyInfo? propertyInfo = null;
            object? propertyValue = null;

            foreach (var part in propertyPathPieces)
            {
                if (propertyInfo == null)
                {
                    propertyInfo = theObject.GetType().GetProperty(part);
                    propertyValue = propertyInfo?.GetValue(theObject) ?? null;
                }

                else if (propertyValue != null)
                {
                    propertyInfo = propertyValue.GetType().GetProperty(part);

                    if (propertyInfo == null)
                        throw new Exception("Property path not found");

                    propertyValue = propertyInfo.GetValue(propertyValue);
                }
            }

            if (propertyInfo == null)
                throw new Exception("Invalid use of property selector ObjectExtension.GetProperty<T>");

            return propertyValue;
        }

        public static object GetProperty<T>(this T theObject, string propertyPath)
        {
            // Property selection for generics behaves better if you work with it in pieces
            //
            var propertyPathPieces = GetPropertyPath(theObject, propertyPath);

            return GetProperty(theObject, propertyPathPieces);
        }

        public static void SetProperty<T>(this T theObject, string propertyPath, object propertyValue)
        {
            // -> Validation
            object? propertyOwner = null;
            var propertyInfo = GetPropertyInfo(theObject, propertyPath, out propertyOwner);

            if (propertyInfo == null)
                throw new Exception("Unable to set property value - unable to recover property info for the provided path and object");

            propertyInfo.SetValue(propertyOwner, propertyValue);
        }


        public static object TryGetProperty<T>(this T theObject, string propertyPath)
        {
            try
            {
                return ObjectExtension.GetProperty(theObject, propertyPath);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Creates formatted string of object data using property reflection
        /// </summary>
        public static string FormatToString<T>(this T theObject)
        {
            var properties = typeof(T).GetProperties();

            var result = string.Join(", ", properties.Select(property => property.Name + "=" + property.GetValue(theObject)?.ToString()));

            return "{" + result + " }";
        }
    }
}
