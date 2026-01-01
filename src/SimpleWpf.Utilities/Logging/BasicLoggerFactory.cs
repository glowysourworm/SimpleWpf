using Microsoft.Extensions.Logging;

namespace SimpleWpf.Utilities.Logging
{
    public class BasicLoggerFactory : ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider)
        {

        }

        public ILogger CreateLogger(string categoryName)
        {
            return new BasicLogger();
        }

        public void Dispose()
        {

        }
    }
}
