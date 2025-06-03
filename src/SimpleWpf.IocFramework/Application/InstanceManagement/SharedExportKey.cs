using System;
using System.Collections.Generic;
using System.Linq;

using SimpleWpf.Extensions;

using SimpleWpf.IocFramework.Application.Attribute;
using SimpleWpf.RecursiveSerializer.Shared;

namespace SimpleWpf.IocFramework.Application.InstanceManagement
{
    /// <summary>
    /// Creates an instance key based on values of the export attribute
    /// </summary>
    internal class SharedExportKey
    {
        readonly int _calculatedHashCode;

        /// <summary>
        /// A collection of all exported types that will share the instance
        /// </summary>
        internal IEnumerable<ExportKey> ExportedTypes { get; private set; }

        internal Type ReflectedType { get; private set; }
        internal InstancePolicy Policy { get; private set; }

        internal SharedExportKey(Type reflectedType, InstancePolicy instancePolicy, IEnumerable<ExportKey> exportedTypeKeys)
        {
            if (!exportedTypeKeys.All(exportKey => exportKey.ReflectedType.Equals(reflectedType)))
                throw new FormattedException("SharedExportKey MUST have ALL REFLECTED TYPES MATCH:  {0}", reflectedType);

            if (!exportedTypeKeys.All(exportKey => exportKey.Policy == instancePolicy))
                throw new FormattedException("SharedExportKey MUST have ALL INSTANCE POLICIES MATCH:  {0}", reflectedType);

            this.ReflectedType = reflectedType;
            this.ExportedTypes = exportedTypeKeys;
            this.Policy = instancePolicy;

            // DON'T NEED THE REFLECTED TYPE FOR THE HASH CODE
            _calculatedHashCode = RecursiveSerializerHashGenerator.CreateSimpleHash(instancePolicy, exportedTypeKeys);
        }

        public override int GetHashCode()
        {
            if (_calculatedHashCode == default(int))
                throw new System.Exception("InstanceKey hash code has not been calculated");

            return _calculatedHashCode;
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }

        public override string ToString()
        {
            return this.ReflectedType.ToString();
        }
    }
}
