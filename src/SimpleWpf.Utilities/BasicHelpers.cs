using System.IO;
using System.Windows;
using System.Windows.Threading;

using AutoMapper;

using SimpleWpf.Native.IO;
using SimpleWpf.Utilities.Logging;
using SimpleWpf.Utilities.RecursiveComparer;

namespace SimpleWpf.Utilities
{
    public static class BasicHelpers
    {
        private readonly static SimpleRecursiveComparer Comparer;

        static BasicHelpers()
        {
            Comparer = new SimpleRecursiveComparer();
        }

        public static IEnumerable<string> FastGetFiles(string baseDirectory, string searchPattern, SearchOption option)
        {
            // Scan directories for files (Use NativeIO for much faster iteration. Less managed memory loading)
            using (var fastDirectory = new FastDirectoryIO(baseDirectory, searchPattern, option))
            {
                return fastDirectory.GetFiles()
                                    .Where(x => !x.IsDirectory)
                                    .Select(x => x.Path)
                                    .ToList();
            }
        }

        public static IEnumerable<FastDirectoryResult> FastGetFileData(string baseDirectory, string searchPattern, bool includeDirectories, SearchOption option)
        {
            // Scan directories for files (Use NativeIO for much faster iteration. Less managed memory loading)
            using (var fastDirectory = new FastDirectoryIO(baseDirectory, searchPattern, option))
            {
                return fastDirectory.GetFiles()
                                    .Where(x => !x.IsDirectory || includeDirectories)
                                    .ToList();
            }
        }

        /// <summary>
        /// Checks to see whether the current managed thread is the dispatcher. Also, checks for application closing.
        /// </summary>
        public static ApplicationIsDispatcherResult IsDispatcher()
        {
            if (Application.Current == null)
                return ApplicationIsDispatcherResult.ApplicationClosing;

            else if (Thread.CurrentThread.ManagedThreadId == Application.Current.Dispatcher.Thread.ManagedThreadId)
                return ApplicationIsDispatcherResult.True;

            else
                return ApplicationIsDispatcherResult.False;
        }

        public static void BeginInvokeDispatcher(Delegate method, DispatcherPriority priority, params object[] parameters)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                Application.Current.Dispatcher.BeginInvoke(method, priority, parameters);

            // Dispatcher (SYNCHRONOUS!)
            else
                method.DynamicInvoke(parameters);
        }

        public static async Task BeginInvokeDispatcherAsync(Delegate method, DispatcherPriority priority, params object[] parameters)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                await Application.Current.Dispatcher.BeginInvoke(method, priority, parameters);

            // Dispatcher (SYNCHRONOUS!)
            else
                method.DynamicInvoke(parameters);
        }

        public static void InvokeDispatcher(Delegate method, DispatcherPriority priority, params object[] parameters)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                Application.Current.Dispatcher.Invoke(method, priority, parameters);

            // Dispatcher
            else
                method.DynamicInvoke(parameters);
        }

        public static TDest Map<TSource, TDest>(TSource source)
        {
            try
            {
                var destination = Activator.CreateInstance(typeof(TDest));

                var mapper = GetMapper<TSource, TDest>();

                return (TDest)mapper.Map(source, destination, typeof(TSource), typeof(TDest));
            }
            catch (Exception ex)
            {
                throw new Exception("Error mapping objects:  BasicHelpers.cs", ex);
            }
        }

        public static void MapOnto<TSource, TDest>(TSource source, TDest dest)
        {
            try
            {
                var mapper = GetMapper<TSource, TDest>();

                mapper.Map(source, dest, typeof(TSource), typeof(TDest));
            }
            catch (Exception ex)
            {
                throw new Exception("Error mapping objects:  BasicHelpers.cs", ex);
            }
        }

        public static bool Compare<T>(T object1, T object2)
        {
            try
            {
                return Comparer.Compare<T>(object1, object2);
            }
            catch (Exception ex)
            {
                throw new Exception("Error comparing objects:  BasicHelpers.cs", ex);
            }
        }

        private static IMapper GetMapper<TSource, TDest>()
        {
            if (ApplicationMapperCache.Has<TSource, TDest>())
                return ApplicationMapperCache.Get<TSource, TDest>();

            try
            {
                var config = new MapperConfiguration(cfg =>
                {
                    var map = cfg.CreateMap<TSource, TDest>();

                }, new BasicLoggerFactory());

                var mapper = config.CreateMapper();

                ApplicationMapperCache.Set<TSource, TDest>(mapper);

                return mapper;
            }
            catch (Exception ex)
            {
                throw new Exception("Error mapping objects:  BasicHelpers.cs", ex);
            }
        }
    }
}
