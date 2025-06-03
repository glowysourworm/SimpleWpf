using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SimpleWpf.Extensions;

namespace SimpleWpf.IocFramework.Application.IocException
{
    /// <summary>
    /// User exception for errors during object creation. Instance factories are one public usage of this
    /// exception type.
    /// </summary>
    public class IocInstanceCreationException : FormattedException
    {
        public IocInstanceCreationException(string messageFormat, params object[] arguments) : base(messageFormat, arguments)
        {
        }

        public IocInstanceCreationException(string messageFormat, Exception exception, params object[] arguments) 
            : base(messageFormat, exception, arguments)
        {
        }
    }
}
