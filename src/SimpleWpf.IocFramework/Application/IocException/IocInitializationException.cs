using System;

using SimpleWpf.Extensions;

namespace SimpleWpf.IocFramework.Application.IocException
{
    internal class IocInitializationException : FormattedException
    {
        public IocInitializationException(string message) : base(message)
        {
        }

        public IocInitializationException(string message, params object[] args) : base(message, args)
        {
        }

        public IocInitializationException(string message, Exception innerException, params object[] args) : base(message, innerException, args)
        {
        }
    }
}
