using Microsoft.Extensions.Logging;

namespace SimpleWpf.Utilities.Logging
{
    public class BasicLogger : ILogger
    {
        /// <summary>
        /// NOTE:  The return value may be the actual logger.. needs to be figured out because I might've caused my
        /// own bug in AudioStation.
        /// </summary>
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
        }
    }
}
