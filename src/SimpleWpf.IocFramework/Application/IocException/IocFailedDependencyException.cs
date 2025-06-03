using System;

using SimpleWpf.Extensions;

namespace SimpleWpf.IocFramework.Application.IocException
{
    internal class IocFailedDependencyException : FormattedException
    {
        public Type FailedDependency { get; set; }

        public IocFailedDependencyException(Type failedDependency, string message) : base(message)
        {
            this.FailedDependency = failedDependency;
        }

        public IocFailedDependencyException(Type failedDependency, string message, params object[] args) : base(message, args)
        {
            this.FailedDependency = failedDependency;
        }

        public IocFailedDependencyException(Type failedDependency, string message, System.Exception innerException, params object[] args) : base(message, innerException, args)
        {
            this.FailedDependency = failedDependency;
        }
    }
}
