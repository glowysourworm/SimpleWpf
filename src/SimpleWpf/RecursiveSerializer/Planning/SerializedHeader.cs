﻿using System.Collections.Generic;

using SimpleWpf.RecursiveSerializer.IO.Data;

namespace SimpleWpf.RecursiveSerializer.Planning
{
    internal class SerializedHeader
    {
        /// <summary>
        /// Type of obejct serialized
        /// </summary>
        internal HashedType SerializedType { get; private set; }

        /// <summary>
        /// Set of all types to be serialized to file
        /// </summary>
        internal Dictionary<int, HashedType> TypeTable { get; private set; }

        /// <summary>
        /// Lookup for property specifications by OBJECT ID
        /// </summary>
        internal Dictionary<int, PropertySpecification> PropertySpecificationLookup { get; private set; }

        /// <summary>
        /// Property specification groups (of OBJECT IDs)
        /// </summary>
        internal Dictionary<PropertySpecification, List<int>> PropertySpecificationGroups { get; private set; }

        internal SerializedHeader(HashedType serializedType,
                                  Dictionary<int, HashedType> typeTable,
                                  Dictionary<PropertySpecification, List<int>> propertySpecificationGroups)
        {
            this.SerializedType = serializedType;
            this.TypeTable = typeTable;
            this.PropertySpecificationLookup = new Dictionary<int, PropertySpecification>();
            this.PropertySpecificationGroups = propertySpecificationGroups;

            foreach (var element in propertySpecificationGroups)
            {
                foreach (var objectId in element.Value)
                {
                    if (!this.PropertySpecificationLookup.ContainsKey(objectId))
                        this.PropertySpecificationLookup.Add(objectId, element.Key);

                    else
                        throw new System.Exception("Duplicate Property Specification found for object:  " + element.Key.ToString());
                }
            }
        }
    }
}
