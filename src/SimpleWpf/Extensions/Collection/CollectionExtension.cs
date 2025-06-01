using System;
using System.Collections.Generic;

namespace SimpleWpf.Extensions.Collection
{
    public static class ListExtension
    {
        /// <summary>
        /// Removes and returns items that match the given predicate.
        /// </summary>
        public static ICollection<T> Remove<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            var removedItems = new List<T>();

            foreach (var item in collection)
            {
                if (predicate(item))
                    removedItems.Add(item);
            }

            foreach (var item in removedItems)
                collection.Remove(item);

            return removedItems;
        }

        /// <summary>
        /// Adds new items to the collection using the default constructor
        /// </summary>
        /// <typeparam name="T">Type of item in collection</typeparam>
        /// <param name="collection">collection instance</param>
        /// <param name="count">How many new items to add</param>
        public static void AddNew<T>(this ICollection<T> collection, int count)
        {
            for (int index = 0; index < count; index++)
            {
                collection.Add((T)typeof(T).Construct());
            }
        }
    }
}
