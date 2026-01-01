using System.Collections.Concurrent;

using AutoMapper;

using SimpleWpf.RecursiveSerializer.Shared;

namespace SimpleWpf.Utilities
{
    internal static class ApplicationMapperCache
    {
        private static ConcurrentDictionary<int, IMapper> Cache;

        static ApplicationMapperCache()
        {
            Cache = new ConcurrentDictionary<int, IMapper>();
        }

        internal static void Set<TSource, TDest>(IMapper mapper)
        {
            var hashCode = CreateHashCode(typeof(TSource), typeof(TDest));

            if (!Cache.ContainsKey(hashCode))
                Cache.AddOrUpdate(hashCode, mapper, (x, y) => y);
        }

        internal static bool Has<TSource, TDest>()
        {
            var hashCode = CreateHashCode(typeof(TSource), typeof(TDest));

            return Cache.ContainsKey(hashCode);
        }

        internal static IMapper Get<TSource, TDest>()
        {
            var hashCode = CreateHashCode(typeof(TSource), typeof(TDest));

            if (Cache.ContainsKey(hashCode))
                return Cache[hashCode];

            else
                throw new ArgumentException("IMapper instance not contained in the cache");
        }

        private static int CreateHashCode(Type sourceType, Type destinationType)
        {
            return RecursiveSerializerHashGenerator.CreateSimpleHash(sourceType, destinationType);
        }
    }
}
