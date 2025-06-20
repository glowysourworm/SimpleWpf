using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using SimpleWpf.RecursiveSerializer.Shared;

namespace SimpleWpf.ObjectMapping
{
    public static class MapperUtility
    {
        private static readonly RecursiveSerializerShallowCopier Copier;

        static MapperUtility()
        {
            MapperUtility.Copier = new RecursiveSerializerShallowCopier();
        }

        /// <summary>
        /// NOTE*** This method is invoked by the ScenarioEditor mapper! Creates a new object of the destination type 
        /// and returns it with copied values from the source type.
        /// </summary>
        public static TDest MapAll<TSource, TDest>(TSource source)
        {
            return MapperUtility.Copier.MapToNew<TSource, TDest>(source, true, false, false, true, propertyInfo => true);
        }

        /// <summary>
        /// Creates a new object of the destination type and returns it with copied values from the source type
        /// </summary>
        public static TDest Map<TSource, TDest>(TSource source, Func<PropertyInfo, bool> predicate)
        {
            return MapperUtility.Copier.MapToNew<TSource, TDest>(source, true, false, false, true, predicate);
        }

        /// <summary>
        /// Updates a given object from a sourc object using property name resolution
        /// </summary>
        public static TDest MapOnto<TSource, TDest>(TSource source, TDest dest, bool ignoreDifferences, Func<PropertyInfo, bool> predicate)
        {
            MapperUtility.Copier.Map(source, dest, ignoreDifferences, !ignoreDifferences, false, true, predicate);

            return dest;
        }
    }
}
