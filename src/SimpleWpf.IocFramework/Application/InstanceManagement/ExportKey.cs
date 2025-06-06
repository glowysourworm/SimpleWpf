﻿using SimpleWpf.IocFramework.Application.Attribute;
using SimpleWpf.RecursiveSerializer.Shared;

namespace SimpleWpf.IocFramework.Application.InstanceManagement
{
    /// <summary>
    /// Creates an instance key based on values of the export attribute
    /// </summary>
    internal class ExportKey
    {
        readonly int _calculatedHashCode;

        internal Type ReflectedType { get; private set; }
        internal Type ExportedType { get; private set; }
        internal InstancePolicy Policy { get; private set; }

        /// <summary>
        /// True if the user has set a specific integer key for the export
        /// </summary>
        internal bool IsKeyed { get; private set; }

        /// <summary>
        /// User-specific integer key for the export
        /// </summary>
        internal int Key { get; private set; }

        internal ExportKey(Type reflectedType, Type exportedType, InstancePolicy instancePolicy, int exportKey, bool isKeyed)
        {
            this.ReflectedType = reflectedType;
            this.ExportedType = exportedType;
            this.Policy = instancePolicy;
            this.Key = exportKey;
            this.IsKeyed = isKeyed;

            // Create a base hash code by using the recursive serializer to gather value types from the System.Type
            var hashCode = RecursiveSerializerHashGenerator.CreateSimpleHash(exportedType,
                                                                             reflectedType,
                                                                             instancePolicy,
                                                                             isKeyed,
                                                                             isKeyed ? exportKey : 0);

            _calculatedHashCode = hashCode;
        }

        public override int GetHashCode()
        {
            if (_calculatedHashCode == default(int))
                throw new System.Exception("InstanceKey hash code has not been calculated");

            return _calculatedHashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (!(obj is ExportKey))
                return false;

            return (obj as ExportKey).GetHashCode() == this.GetHashCode();
        }

        public override string ToString()
        {
            return this.ExportedType.ToString();
        }
    }
}
