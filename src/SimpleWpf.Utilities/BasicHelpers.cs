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
        private readonly static Dispatcher MainThread;

        static BasicHelpers()
        {
            Comparer = new SimpleRecursiveComparer();
            MainThread = Application.Current.Dispatcher;
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

        /*
        
            Invoking the Dispatcher:  Different methods are needed depending on the situation. There are a couple pitfalls:

                                      1) The application's pointer goes null before the application exits
                                      2) DynamicInvoke does NOT ensure same thread will be called before it executes the Delegate!!!

        */

        public static void BeginInvokeDispatcher(Action method, DispatcherPriority priority)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                Application.Current.Dispatcher.BeginInvoke(method, priority);

            // Dispatcher (Must invoke this thread!)(DynamicInvoke does not guarantee invoke thread is the same!!!)
            else
                method.BeginInvoke(null, null);
        }
        public static void BeginInvokeDispatcher<T1>(Action<T1> method, DispatcherPriority priority, T1 parameter1)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                Application.Current.Dispatcher.BeginInvoke(method, priority, parameter1);

            // Dispatcher (Must invoke this thread!)(DynamicInvoke does not guarantee invoke thread is the same!!!)
            else
                method.BeginInvoke(parameter1, null, null);
        }
        public static void BeginInvokeDispatcher<T1, T2>(Action<T1, T2> method, DispatcherPriority priority, T1 parameter1, T2 parameter2)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                Application.Current.Dispatcher.BeginInvoke(method, priority, parameter1, parameter2);

            // Dispatcher (Must invoke this thread!)(DynamicInvoke does not guarantee invoke thread is the same!!!)
            else
                method.BeginInvoke(parameter1, parameter2, null, null);
        }
        public static void BeginInvokeDispatcher<T1, T2, T3>(Action<T1, T2, T3> method, DispatcherPriority priority, T1 parameter1, T2 parameter2, T3 parameter3)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                Application.Current.Dispatcher.BeginInvoke(method, priority, parameter1, parameter2, parameter3);

            // Dispatcher (Must invoke this thread!)(DynamicInvoke does not guarantee invoke thread is the same!!!)
            else
                method.BeginInvoke(parameter1, parameter2, parameter3, null, null);
        }

        public static void InvokeDispatcher(Action method, DispatcherPriority priority)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                Application.Current.Dispatcher.Invoke(method, priority);

            // Dispatcher
            else
                method.Invoke();
        }
        public static void InvokeDispatcher<T1>(Action<T1> method, DispatcherPriority priority, T1 parameter1)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                Application.Current.Dispatcher.Invoke(method, priority, parameter1);

            // Dispatcher
            else
                method.Invoke(parameter1);
        }
        public static void InvokeDispatcher<T1, T2>(Action<T1, T2> method, DispatcherPriority priority, T1 parameter1, T2 parameter2)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                Application.Current.Dispatcher.Invoke(method, priority, parameter1, parameter2);

            // Dispatcher
            else
                method.Invoke(parameter1, parameter2);
        }
        public static void InvokeDispatcher<T1, T2, T3>(Action<T1, T2, T3> method, DispatcherPriority priority, T1 parameter1, T2 parameter2, T3 parameter3)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                Application.Current.Dispatcher.Invoke(method, priority, parameter1, parameter2, parameter3);

            // Dispatcher
            else
                method.Invoke(parameter1, parameter2, parameter3);
        }

        public static TResult InvokeDispatcher<TResult>(Func<TResult> method, DispatcherPriority priority)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                return Application.Current.Dispatcher.Invoke<TResult>(method, priority);

            // Dispatcher
            else
                return (TResult)method.Invoke();
        }
        public static TResult InvokeDispatcher<T1, TResult>(Func<T1, TResult> method, DispatcherPriority priority, T1 parameter1)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                return (TResult)Application.Current.Dispatcher.Invoke(method, priority, parameter1);

            // Dispatcher
            else
                return (TResult)method.Invoke(parameter1);
        }
        public static TResult InvokeDispatcher<T1, T2, TResult>(Func<T1, T2, TResult> method, DispatcherPriority priority, T1 parameter1, T2 parameter2)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                return (TResult)Application.Current.Dispatcher.Invoke(method, priority, parameter1);

            // Dispatcher
            else
                return (TResult)method.Invoke(parameter1, parameter2);
        }
        public static TResult InvokeDispatcher<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> method, DispatcherPriority priority, T1 parameter1, T2 parameter2, T3 parameter3)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                return (TResult)Application.Current.Dispatcher.Invoke(method, priority, parameter1);

            // Dispatcher
            else
                return (TResult)method.Invoke(parameter1, parameter2, parameter3);
        }

        public static async void BeginInvokeDispatcherAsyncAwait(Delegate method, DispatcherPriority priority, params object[] parameters)
        {
            await MainThread.BeginInvoke(method, priority, parameters);
        }
        public static async void BeginInvokeDispatcherAsyncAwait(Action method, DispatcherPriority priority)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                await Application.Current.Dispatcher.BeginInvoke(method, priority);

            // Dispatcher
            else
            {
                var wait = method.BeginInvoke(null, null);

                while (!wait.IsCompleted)
                {
                    Thread.Sleep(1);
                }
            }
        }
        public static async void BeginInvokeDispatcherAsyncAwait<T1>(Action<T1> method, DispatcherPriority priority, T1 parameter1)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                await Application.Current.Dispatcher.BeginInvoke(method, priority, parameter1);

            // Dispatcher
            else
            {
                var wait = method.BeginInvoke(parameter1, null, null);

                while (!wait.IsCompleted)
                {
                    Thread.Sleep(1);
                }
            }
        }
        public static async void BeginInvokeDispatcherAsyncAwait<T1, T2>(Action<T1, T2> method, DispatcherPriority priority, T1 parameter1, T2 parameter2)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                await Application.Current.Dispatcher.BeginInvoke(method, priority, parameter1, parameter2);

            // Dispatcher
            else
            {
                var wait = method.BeginInvoke(parameter1, parameter2, null, null);

                while (!wait.IsCompleted)
                {
                    Thread.Sleep(1);
                }
            }
        }
        public static async void BeginInvokeDispatcherAsyncAwait<T1, T2, T3>(Action<T1, T2, T3> method, DispatcherPriority priority, T1 parameter1, T2 parameter2, T3 parameter3)
        {
            if (IsDispatcher() == ApplicationIsDispatcherResult.False)
                Application.Current.Dispatcher.Invoke(method, priority, parameter1, parameter2, parameter3);

            // Dispatcher
            else
            {
                var wait = method.BeginInvoke(parameter1, parameter2, parameter3, null, null);

                while (!wait.IsCompleted)
                {
                    Thread.Sleep(1);
                }
            }
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
