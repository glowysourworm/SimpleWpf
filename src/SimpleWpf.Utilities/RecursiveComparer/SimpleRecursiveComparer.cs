using System.Collections;
using System.Reflection;

using AutoMapper.Internal;

using SimpleWpf.Extensions;
using SimpleWpf.Utilities.RecursiveComparer.Attribute;
using SimpleWpf.Utilities.RecursiveComparer.Interface;

namespace SimpleWpf.Utilities.RecursiveComparer
{
    public class SimpleRecursiveComparer : IRecursiveComparer
    {
        public SimpleRecursiveComparer()
        {
        }

        public bool Compare<T>(T object1, T object2)
        {
            var result = true;

            // Check Recursion:  This may be a recursion leaf. So, check to see if we've finished.
            //
            var bothNulls = false;

            if (!VerifyReferences(object1, object2, out bothNulls))
                return false;

            if (bothNulls)
                return true;

            // Get all (shallow) properties for this object
            var properties = ReflectionStore.GetAll<T>();

            // CHECK FOR PRIMITIVES!
            if (!properties.Any())
            {
                // This could be cleaned up a bit.. when we find all possibilities for these value types
                //
                if (!IsPrimitive(typeof(T)) && (typeof(T) != typeof(string)))
                    throw new Exception("Improper use of primitives:  SimpleRecursiveComparer.cs");

                return object1.Equals(object2);
            }

            foreach (var property in properties)
            {
                // IGNORED PROPERTIES
                if (property.CustomAttributes.Any(x => x.AttributeType == typeof(RecursiveCompareIgnoreAttribute)))
                    continue;

                result &= CompareRecurse(object1, object2, property);

                if (!result)
                    break;
            }

            return result;
        }

        public bool CompareRecurse<T>(T object1, T object2, PropertyInfo property)
        {
            // Best to enumerate these types. MSFT's Type (System.Runtime.Type) is not easy to pick apart; and
            // there are properties that don't make sense. IsPrimitive, for example, will miss some of these 
            // "blue" fields.
            //

            // Nullable
            if (IsNullable(property.PropertyType))
                return CompareNullable(object1, object2, property);

            // Primitives
            else if (IsPrimitive(property.PropertyType))
                return ComparePrimitive(object1, object2, property);

            // string (also, non-primitive) (TODO)
            else if (property.PropertyType == typeof(string))
                return ComparePrimitive(object1, object2, property);

            // Any IComparable
            else if (property.PropertyType.HasInterface<IComparable>())
                return CompareIComparable<T>(object1, object2, property);

            // Collections
            else if (property.PropertyType.HasInterface<IEnumerable>())
                return CompareCollection(object1, object2, property);

            else if (property.PropertyType.HasInterface<IDictionary>())
                return CompareCollection(object1, object2, property);

            // Complex Types
            else
                return CompareReference(object1, object2, property);
        }

        public bool CompareDirectly<T>(T object1, T object2)
        {
            // Compare Directly:  This is used to iterate an object by inspecting its type, where we need 
            //                    information before calling our recursive entry point. (which we can avoid doing).
            //
            // CompareRecurse (see above)
            //

            // VERIFY REFERENCES!
            var bothNulls = false;

            if (!VerifyReferences(object1, object2, out bothNulls))
                return false;

            if (bothNulls)
                return true;

            // Get "property" type
            var type = object1.GetType();

            // Nullable (Go ahead and recurse)
            if (IsNullable(type))
                return MakeGenericCompareCall(object1, object2);

            // Primitives
            else if (IsPrimitive(type))
                return object1.Equals(object2);

            // string (also, non-primitive) (TODO)
            else if (type == typeof(string))
                return object1.Equals(object2);

            // Any IComparable
            else if (type.HasInterface<IComparable>())
                return MakeGenericCompareCall(object1, object2);

            // Collections
            else if (type.HasInterface<IEnumerable>())
                return MakeGenericCompareCall(object1, object2);

            else if (type.HasInterface<IDictionary>())
                return MakeGenericCompareCall(object1, object2);

            // Complex Types
            else
                return MakeGenericCompareCall(object1, object2);
        }

        private bool IsNullable(Type type)
        {
            return type.IsGenericType(typeof(Nullable<>));
        }

        private bool IsPrimitive(Type type)
        {
            if (type == typeof(bool))
                return true;

            else if (type == typeof(byte))
                return true;

            else if (type == typeof(short))
                return true;

            else if (type == typeof(char))
                return true;

            else if (type == typeof(int))
                return true;

            else if (type == typeof(long))
                return true;

            else if (type == typeof(double))
                return true;

            else if (type == typeof(float))
                return true;

            else if (type == typeof(uint))
                return true;

            else if (type == typeof(ushort))
                return true;

            else if (type == typeof(ulong))
                return true;

            else if (type.IsEnum)
                return true;

            return false;
        }

        private bool CompareNullable<TObject>(TObject object1, TObject object2, PropertyInfo propertyInfo)
        {
            var nullable1 = ReflectReference<TObject>(object1, propertyInfo);
            var nullable2 = ReflectReference<TObject>(object2, propertyInfo);
            var bothNulls = false;

            if (!VerifyReferences(nullable1, nullable2, out bothNulls))
                return false;

            if (bothNulls)
                return true;

            // PRIMITIVE VALUE (ComparePrimitive)
            if (IsPrimitive(nullable1.GetType()))
                return nullable1.Equals(nullable2);

            // Recurse (will compare Nullable sub-properties)
            return Compare(nullable1, nullable2);
        }
        private bool ComparePrimitive<TObject>(TObject object1, TObject object2, PropertyInfo propertyInfo)
        {
            var value1 = ReflectPrimitive<TObject>(object1, propertyInfo);
            var value2 = ReflectPrimitive<TObject>(object2, propertyInfo);

            // VERIFY STRING "PRIMITIVE" NULL REFERENCES! (this can't hurt; but there's obviously some badly needed type grouping)
            var bothNulls = false;

            if (!VerifyReferences(value1, value2, out bothNulls))
                return false;

            if (bothNulls)
                return true;

            return value1.Equals(value2);
        }

        private bool CompareReference<TObject>(TObject object1, TObject object2, PropertyInfo propertyInfo)
        {
            var value1 = ReflectReference<TObject>(object1, propertyInfo);
            var value2 = ReflectReference<TObject>(object2, propertyInfo);
            var bothNulls = false;

            if (!VerifyReferences(value1, value2, out bothNulls))
                return false;

            if (bothNulls)
                return true;

            // Recurse
            return Compare(value1, value2);
        }

        private bool CompareIComparable<TObject>(TObject object1, TObject object2, PropertyInfo propertyInfo)
        {
            var value1 = (IComparable)ReflectReference<TObject>(object1, propertyInfo);
            var value2 = (IComparable)ReflectReference<TObject>(object2, propertyInfo);
            var bothNulls = false;

            if (!VerifyReferences(value1, value2, out bothNulls))
                return false;

            if (bothNulls)
                return true;

            return value1.CompareTo(value2) == 0;
        }

        private bool CompareCollection<TObject>(TObject object1, TObject object2, PropertyInfo propertyInfo)
        {
            var value1 = (IEnumerable)ReflectReference<TObject>(object1, propertyInfo);
            var value2 = (IEnumerable)ReflectReference<TObject>(object2, propertyInfo);
            var bothNulls = false;

            if (!VerifyReferences(value1, value2, out bothNulls))
                return false;

            if (bothNulls)
                return true;

            var value1Copy = new ArrayList();
            var value2Copy = new ArrayList();
            var value1Count = 0;
            var value2Count = 0;

            // Cut down on iteration. Just get it out of the way...There's a hidden iterator each step of the way.
            // 
            foreach (var item in value1)
            {
                value1Copy.Add(item);
                value1Count++;
            }
            foreach (var item in value2)
            {
                value2Copy.Add(item);
                value2Count++;

                // Use this loop to compare the items, also
                if (value2Count > value1Count)
                    return false;

                var item1 = value1Copy[value2Count - 1];
                var item2 = value2Copy[value2Count - 1];

                // Recurse:  Must add type information to this recursive invoke statement. The "template"
                //           type is not seen by the CLR as "string" (for example). It will see "object".
                //
                //           So, recurse using CompareDirectly, which will handle this inspection.
                //
                var itemCompare = CompareDirectly(item1, item2);

                if (!itemCompare)
                    return false;
            }

            if (value1Count != value2Count)
                return false;

            return true;
        }

        // Check possible null combinations
        private bool VerifyReferences<TObject>(TObject object1, TObject object2, out bool bothNulls)
        {
            bothNulls = false;

            if (object1 == null && object2 == null)
            {
                bothNulls = true;
                return true;
            }

            else if (object1 != null && object2 == null)
                return false;

            else if (object1 == null && object2 != null)
                return false;

            return true;
        }

        private object ReflectPrimitive<TObject>(TObject theObject, PropertyInfo propertyInfo)
        {
            try
            {
                return propertyInfo.GetValue(theObject);
            }
            catch (Exception ex)
            {
                throw new Exception("Error reflecting primitive:  SimpleWpf.Utilities.SimpleRecursiveComparer", ex);
            }
        }

        private object ReflectReference<TObject>(TObject theObject, PropertyInfo propertyInfo)
        {
            try
            {
                if (theObject == null)
                    return default;

                return propertyInfo.GetValue(theObject);
            }
            catch (Exception ex)
            {
                throw new Exception("Error reflecting reference:  SimpleWpf.Utilities.SimpleRecursiveComparer", ex);
            }
        }

        /// <summary>
        /// Recursively invokes the primary input method "Compare" using reflection to make a generic call
        /// based on object type
        /// </summary>
        private bool MakeGenericCompareCall(object object1, object object2)
        {
            try
            {
                var bothNulls = false;

                if (!VerifyReferences(object1, object2, out bothNulls))
                    return false;

                if (bothNulls)
                    return true;

                var methodInfo = this.GetType().GetMethod("Compare");
                var genericMethod = methodInfo.MakeGenericMethod(object1.GetType());

                return (bool)genericMethod.Invoke(this, new object[] { object1, object2 });
            }
            catch (Exception ex)
            {
                throw new Exception("Error trying to recure into comparer:  " + ex.Message, ex);
            }
        }
    }
}
