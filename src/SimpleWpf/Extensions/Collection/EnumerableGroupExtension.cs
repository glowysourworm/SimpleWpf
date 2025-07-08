using System.Linq.Expressions;
using System.Reflection;

namespace SimpleWpf.Extensions.Collection
{
    /// <summary>
    /// These methods will be extensions to deal with group situations involving IEnumerable.
    /// </summary>
    public static class EnumerableGroupExtension
    {
        /// <summary>
        /// Returns a property that must be valid for the group. This means that the collection must agree on the property's
        /// value, otherwise there will be a default returned.
        /// </summary>
        public static TResult GetGroupProperty<T, TResult>(this IEnumerable<T> collection,
                                                           Func<T, TResult> selector,
                                                           TResult defaultValue = default(TResult))
        {
            TResult result = defaultValue;

            foreach (var item in collection)
            {
                var nextValue = selector(item);

                if (result == null &&
                    nextValue == null)
                    continue;

                else if (result == null &&
                         nextValue != null)
                    return defaultValue;

                else if (result != null &&
                         nextValue == null)
                    return defaultValue;

                else if (result.Equals(defaultValue))
                    result = nextValue;

                else if (!nextValue.Equals(result))
                    return defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Sets a property that for the group. NOTE:  The member expression must point to a property!
        /// </summary>
        public static void SetGroupProperty<T, TResult>(this IEnumerable<T> collection, TResult setValue, Expression<Func<T, TResult>> propertyExpression)
        {
            var lambda = propertyExpression.Body as MemberExpression;

            if (lambda != null &&
                lambda.NodeType == ExpressionType.MemberAccess)
            {
                var propertyInfo = lambda.Member as PropertyInfo;

                foreach (var item in collection)
                {
                    propertyInfo.SetValue(item, setValue);
                }
            }
            else
                throw new ArgumentException("Expression must be a valid lambda expression for a class property");
        }
    }
}
