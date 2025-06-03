using System;

namespace SimpleWpf.IocFramework.Application.InstanceManagement.Interface
{
    /// <summary>
    /// The IocInstanceFactory component creates a user defined component that allows the
    /// user to build component pieces from other resources before the container uses it.
    /// </summary>
    public interface IIocInstanceFactory<T>
    {
        /// <summary>
        /// Returns instance of user component.
        /// </summary>
        T CreateInstance();
    }
}
