using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using SimpleWpf.IocFramework.Application.Attribute;
using SimpleWpf.IocFramework.Application.IocException;
using SimpleWpf.RecursiveSerializer.Shared;

namespace SimpleWpf.IocFramework.Application.InstanceManagement
{
    /// <summary>
    /// Simple reflection based instance factory. This will be the default factory for the
    /// Ioc backend. The instance factory interface may be used for user defined factories.
    /// </summary>
    public class InstanceFactory
    {
        /// <summary>
        /// Type to construct. This should be the reflected type of the export - the actual type. The
        /// exported type is usually an interface that it implements; or the base class.
        /// </summary>
        internal Type DesiredType { get; }

        // Backend
        internal ConstructorInfo Ctor { get; private set; }
        internal ParameterInfo[] Parameters { get; private set; }

        // User - Parameterless Method
        internal MethodInfo UserMethod { get; private set; }
        internal bool IsUserFactory { get; }

        /// <summary>
        /// INTERNAL USE ONLY: This constructor will set up the parameter-constructor info for the 
        ///                    primary object graph resolution code path. Parameters must be resovled
        ///                    using the ExportFactory recursive code. The instance type will be used
        ///                    to locate constructor info.
        /// </summary>
        public InstanceFactory(Type desiredType)
        {
            this.DesiredType = desiredType;
            this.IsUserFactory = false;

            ResolveConstructor(desiredType);
        }

        /// <summary>
        /// USER CODE ALLOWED: This constructor will allow for a user instance factory. The type must
        ///                    inherit from IIocInstanceFactory. A resolved object (factory) must be
        ///                    provided during usage for object resolution.
        /// </summary>
        public InstanceFactory(Type desiredType, Type factoryType)
        {
            this.DesiredType = desiredType;
            this.IsUserFactory = true;

            ResolveUserConstructor(desiredType, factoryType);
        }

        public object CreateInstance(object userFactory = null, params object[] injectedParameters)
        {
            // User
            if (this.IsUserFactory)
            {
                if (userFactory == null)
                    throw new ArgumentException("Invalid use of InstanceFactory.CreateInstance:  User code path not honored");

                try
                {
                    return this.UserMethod.Invoke(userFactory, new object[] { });
                }
                catch (Exception ex)
                {
                    throw new IocInstanceCreationException("Error trying to invoke constructor for type {0} (Must have default constructor or one with the IocImportingConstructor attribute", ex,
                                                            this.DesiredType.FullName);
                }
            }

            // Backend
            else
            {
                try
                {
                    return this.Ctor.Invoke(injectedParameters);
                }
                catch (Exception ex)
                {
                    throw new IocInstanceCreationException("Error trying to invoke constructor for type {0} (Must have default constructor or one with the IocImportingConstructor attribute", ex,
                                                            this.DesiredType.FullName);
                }
            }
        }

        private void ResolveUserConstructor(Type desiredType, Type factoryType)
        {
            try
            {
                // Get IIocInstanceFactory.CreateInstance method
                var instanceInterfaceType = factoryType.GetInterface("IIocInstanceFactory");

                var userMethod = instanceInterfaceType.GetMethod("CreateInstance");

                this.UserMethod = userMethod.MakeGenericMethod(desiredType);
            }
            catch (Exception ex)
            {
                throw new IocInstanceCreationException("Error trying to locate CreateInstance method for type {0}", ex, factoryType.FullName);
            }
        }

        private void ResolveConstructor(Type desiredType)
        {
            IEnumerable<ConstructorInfo> constructors;

            // Constructors
            try
            {
                constructors = desiredType.GetConstructors();
            }
            catch (Exception ex)
            {
                throw new IocInstanceCreationException("Error trying to locate constructor for type {0}", ex, desiredType.FullName);
            }

            // Importing Constructor
            try
            {
                this.Ctor = constructors.SingleOrDefault(x => x.IsDefined(typeof(IocImportingConstructorAttribute))) ??
                            constructors.First(x => x.GetParameters().Length == 0);

                this.Parameters = this.Ctor.GetParameters();
            }
            catch (Exception ex)
            {
                throw new IocInstanceCreationException("Error trying to locate constructor for type {0} (Must have default constructor or one with the IocImportingConstructor attribute)", ex,
                                                       desiredType.FullName);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (!(obj is InstanceFactory))
                return false;

            return (obj as InstanceFactory).GetHashCode() == this.GetHashCode();
        }

        public override int GetHashCode()
        {
            return RecursiveSerializerHashGenerator.CreateSimpleHash(this.DesiredType, this.IsUserFactory);
        }
    }
}
