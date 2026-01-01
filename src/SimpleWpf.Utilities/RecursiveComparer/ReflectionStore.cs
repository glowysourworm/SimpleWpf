using System.Collections.Concurrent;
using System.Reflection;

using SimpleWpf.RecursiveSerializer.Shared;

namespace SimpleWpf.Utilities.RecursiveComparer
{
    /// <summary>
    /// This class needs to share with the RecursiveSerializer - which should own the
    /// logic for this comparer.
    /// </summary>
    public static class ReflectionStore
    {
        private static readonly ConcurrentDictionary<int, PropertyInfo> Properties;

        static ReflectionStore()
        {
            Properties = new ConcurrentDictionary<int, PropertyInfo>();
        }

        public static IEnumerable<PropertyInfo> GetAll<T>()
        {
            return CreateProperties<T>();
        }

        public static PropertyInfo Get<T>(string propertyName)
        {
            var hashCode = CreateHashCode<T>(propertyName);

            if (Properties.ContainsKey(hashCode))
                return Properties[hashCode];

            // Create Properties for this type
            CreateProperties<T>();

            if (!Properties.ContainsKey(hashCode))
                throw new Exception(string.Format("Error creating properties for type {0}", typeof(T).Name));

            return Properties[hashCode];
        }

        private static IEnumerable<PropertyInfo> CreateProperties<T>()
        {
            try
            {
                var propertyInfo = typeof(T).GetProperties();

                foreach (var info in propertyInfo)
                {
                    // Using PropertyType (not DeclaringType) (also, full type name with assembly)
                    var hashCode = CreateHashCode<T>(info.Name);

                    if (!Properties.ContainsKey(hashCode))
                        Properties.AddOrUpdate(hashCode, info, (x, y) => y);
                }

                return propertyInfo;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error reflecting type {0}", typeof(T).Name));
            }
        }

        private static int CreateHashCode<T>(string propertyName)
        {
            return RecursiveSerializerHashGenerator.CreateSimpleHash(propertyName, typeof(T).AssemblyQualifiedName);
        }
    }
}
