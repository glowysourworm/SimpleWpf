﻿using System.Reflection;

using SimpleWpf.Extensions.Collection;
using SimpleWpf.IocFramework.Application.Attribute;
using SimpleWpf.IocFramework.Application.IocException;

namespace SimpleWpf.IocFramework.Application.InstanceManagement
{
    /// <summary>
    /// Creates the instance cache required for dependency injection
    /// </summary>
    internal static class ExportFactory
    {
        /// <summary>
        /// Creates primary cache for the provided assemblies
        /// </summary>
        internal static ExportCache CreateCache(IEnumerable<Assembly> assemblies)
        {
            var cache = new ExportCache();
            var exportKeys = new List<ExportKey>();
            var sharedExportKeys = new List<SharedExportKey>();

            // Gather exports from the assemblies
            var iocExportTypes = assemblies.SelectMany(assembly => assembly.GetExportedTypes())
                                           .Where(type => type.IsDefined(typeof(IocExportAttribute)) ||
                                                          type.IsDefined(typeof(IocExportDefaultAttribute)) ||
                                                          type.IsDefined(typeof(IocExportSpecificAttribute)))
                                           .Actualize();

            // Use these after exports are resolved to store instance factory relationships
            var iocAttributes = new List<IocExportBaseAttribute>();

            // Create export type keys from the export types
            foreach (var type in iocExportTypes)
            {
                // NOTE*** CHECK FOR MULTIPLE EXPORTS!!
                //
                foreach (var attribute in type.GetCustomAttributes<IocExportBaseAttribute>())
                {

                    // Create the InstanceFactory: This factory is built on a simple pattern that has
                    //                             hooks for a parameterless USER CTOR. This ctor would
                    //                             give control to user code using the IIocInstanceFactory
                    //                             interface. The option is provided in the attribute
                    //                             hierarchy - ExportFactoryType.
                    //
                    //                             Otherwise, the default is to use the InstanceFactory
                    //                             wrapper class to select the proper code path. Dependencies
                    //                             for injection are provided to this class - but only for
                    //                             backend (internal) instance creation.
                    //
                    //                             Dependency injection for user constructors would require
                    //                             more careful reworking of building the dependency chain
                    //                             before entering the recursion loop. 

                    // Save these for after the exports are resolved
                    iocAttributes.Add(attribute);

                    if (attribute is IocExportAttribute)
                    {
                        var exportAttrib = (IocExportAttribute)attribute;

                        // Add export type key for the [IocExport]
                        exportKeys.Add(new ExportKey(type, exportAttrib.ExportType ?? type, exportAttrib.InstancePolicy, 0, false));
                    }
                    else if (attribute is IocExportDefaultAttribute)
                    {
                        var exportAttrib = (IocExportDefaultAttribute)attribute;

                        // Add export type key for the [IocExportDefault] (SHARED + EXPORT TYPE = REFLECTED TYPE)
                        exportKeys.Add(new ExportKey(type, type, exportAttrib.InstancePolicy, 0, false));
                    }
                    else if (attribute is IocExportSpecificAttribute)
                    {
                        var exportAttrib = (IocExportSpecificAttribute)attribute;

                        // Add export type key for the [IocExportSpecific] (SHARED + KEYED)
                        exportKeys.Add(new ExportKey(type, exportAttrib.ExportType ?? type, exportAttrib.InstancePolicy, exportAttrib.ExportKey, true));
                    }
                    else
                        throw new Exception("Unhandled IocExportAttributeBase type in ExportFactory.CreateCache");
                }
            }

            // Create shared export type keys
            var reflectedTypeGroups = exportKeys.GroupBy(exportKey => exportKey.ReflectedType);

            foreach (var group in reflectedTypeGroups)
            {
                var anyGlobalShared = group.Any(x => x.Policy == InstancePolicy.ShareGlobal);

                // VALIDATE SHARED EXPORTS
                if (anyGlobalShared && !group.All(x => x.Policy == InstancePolicy.ShareGlobal))
                    throw new IocInitializationException("Invalid InstancePolicy for type {0}. For types marked ShareGlobal, ALL exports must obey the same policy!", group.Key);

                else if (!anyGlobalShared && !group.Any(x => x.Policy == InstancePolicy.ShareExportedType))
                    continue;

                var exportKey = group.First();

                sharedExportKeys.Add(new SharedExportKey(group.Key, anyGlobalShared ? InstancePolicy.ShareGlobal
                                                                                    : InstancePolicy.ShareExportedType,
                                                         group.Where(x => x.Policy == exportKey.Policy)));
            }

            // Iterate to resolve and cache the exports
            foreach (var exportTypeKey in exportKeys)
            {
                // Procedure:  ALL EXPORTS ARE CACHED - COLLISIONS ARE EXPOSED USING THE ExportTypeKey
                //
                // 1) If export has been worked on, Then continue
                // 2) Construct the current item: Instance (GRAPH!)
                // 3) Take dependencies and keep them in the working collection (Just use the cache)
                // 4) Continue iteration and skip items that have been created
                //

                // Completed during dependency graph resolution
                if (cache.HasExport(exportTypeKey))
                    continue;

                // Resolve the export (Caches the instance due to recursion)
                var export = ResolveExport(exportTypeKey, new List<ExportKey>(), exportKeys, sharedExportKeys, cache);

                if (export.Policy == InstancePolicy.ShareGlobal ||
                    export.Policy == InstancePolicy.ShareExportedType)
                {
                    var sharedExportKey = sharedExportKeys.FirstOrDefault(sharedKey => sharedKey.ExportedTypes.Contains(exportTypeKey));

                    if (sharedExportKey == null)
                        throw new IocInitializationException("Invalid or missing export key:  ExportFactory.cs");

                    cache.SetExport(exportTypeKey, export, sharedExportKey);
                }
                else
                    cache.SetExport(exportTypeKey, export, null);
            }

            // Setup instance factory relationships
            foreach (var attribute in iocAttributes)
            {
                // Store export keys to utilize instance factories
                if (attribute.ExportFactoryType != null)
                {
                    var exportId = ((IocExportSpecificAttribute)attribute)?.ExportKey ?? 0;
                    var exportIdUsed = attribute is IocExportSpecificAttribute;
                    var export = cache.FindExport(attribute.ExportType, exportId, exportIdUsed);

                    var exportFactoryId = ((IocExportSpecificAttribute)attribute)?.ExportKey ?? 0;
                    var exportFactoryIdUsed = attribute is IocExportSpecificAttribute;
                    var exportFactory = cache.FindExport(attribute.ExportFactoryType, attribute.ExportFactoryInstanceId, attribute.ExportFactoryInstanceSpecific);

                    cache.SetInstanceFactoryRelationship(cache.GetExportKey(export), cache.GetExportKey(exportFactory));
                }
            }

            return cache;
        }

        /// <summary>
        /// Resolves export graph for the provided export using the working cache of exports; and returns the
        /// dependencies ready for injection. (DEPENDENCY HISTORY IS USED FOR RECURSION TO SPOT CIRCULAR REFERENCES)
        /// </summary>
        /// <param name="exportInstanceFactoryKey">Already resolved instance factory export</param>
        /// <returns>Export ready to be constructed</returns>
        private static Export ResolveExport(ExportKey exportTypeKey,
                                            List<ExportKey> dependencyHistory,
                                            IEnumerable<ExportKey> exportTypeKeys,
                                            IEnumerable<SharedExportKey> sharedExportKeys,
                                            ExportCache cache)
        {
            // Procedure
            //
            // 0) Add export to history
            // 1) Check the cache to see if this type has been constructed
            // 2) Find the ctor for the export
            //      -> Check cache to see whether Ctor has been constructed
            // 3) Locate dependencies
            //      -> MUST CHECK [IocImport] FOR INJECTED PARAMETERS
            //      -> CHECK FOR CIRCULAR DEPENDENCIES!
            // 4) Construct new instance graphs for non-cached exports
            //

            // ADD EXPORT TO DEPENDENCY HISTORY
            dependencyHistory.Add(exportTypeKey);

            if (cache.HasExport(exportTypeKey))
                return cache.GetExport(exportTypeKey);

            // Instance Factory:  This is a class (export) that has the responsibility of building
            //                    instances of another class (export). So, both dependency trees are
            //                    resolved independently; and the instance factory is stored with the
            //                    export. (This could be changed to remove worry about the dependencies, 
            //                    because they're actually independent). So, the InstanceFactory (default)
            //                    is the class that will be used to locate parameter dependencies (here).
            //
            //                    At runtime, the container will decide whether to use the default 
            //                    InstanceFactory; or the user instance factory.
            //

            // Dependencies to inject into the constructor
            var instanceFactory = new InstanceFactory(exportTypeKey.ReflectedType);
            var injectedParameters = instanceFactory.Parameters;

            var dependencies = new List<Export>();

            // Resolve the depencies to inject
            foreach (var parameter in injectedParameters)
            {
                // *** NOTE:  MUST LOOK FOR IMPORT ATTRIBUTES ON THE PARAMETERS
                //

                // Resolve the export type key for the dependency
                var dependencyKey = ResolveDependency(parameter, exportTypeKeys);

                // CIRCULAR DEPENDENCIES!
                if (dependencyHistory.Any(x => x.Equals(dependencyKey)))
                    throw new IocCircularDependencyException(exportTypeKey, dependencyHistory, "(Ioc Export / Import) Circular Dependency Detected!");

                // Check for a previously cached export
                var depedencyExport = cache.HasExport(dependencyKey) ? cache.GetExport(dependencyKey) : null;

                // RECURSIVE:  Resolve the dependency if it is a new export. 
                //
                //             NOTE*** This will start a new dependency history - allowing cycles inside the
                //                     dependency tree. This means that only one generation of dependencies 
                //                     is protected. 
                //      
                //                     This should probably be by design since applications allow cyclical graphs.
                //
                if (depedencyExport == null)
                    depedencyExport = ResolveExport(dependencyKey, new List<ExportKey>(), exportTypeKeys, sharedExportKeys, cache);

                // CACHE INTERMEDIATE INSTANCES (DEPENDENCY IS NOW RESOLVED)
                if (!cache.HasExport(dependencyKey))
                {
                    if (depedencyExport.Policy == InstancePolicy.ShareGlobal ||
                        depedencyExport.Policy == InstancePolicy.ShareExportedType)
                    {
                        var sharedExportKey = sharedExportKeys.FirstOrDefault(sharedKey => sharedKey.ExportedTypes.Contains(dependencyKey));

                        if (sharedExportKey == null)
                            throw new IocInitializationException("Invalid or missing export key:  ExportFactory.cs");

                        cache.SetExport(dependencyKey, depedencyExport, sharedExportKey);
                    }
                    else
                        cache.SetExport(dependencyKey, depedencyExport, null);
                }

                dependencies.Add(depedencyExport);
            }

            // Export is now resolved - BUT MAY HAVE OTHER SHARED EXPORTS TO ADD
            return new Export(exportTypeKey, dependencies);
        }

        /// <summary>
        /// Locates and resolves the instance starting with the import dependency. This is located from either the 
        /// instance cache or the export types from the assemblies.
        /// </summary>
        private static ExportKey ResolveDependency(ParameterInfo dependency,
                                                   IEnumerable<ExportKey> exportTypeKeys)
        {
            // Procedure
            //
            // 0) Check the ParameterInfo for an [IocImport] attribute
            // 1) Resolve the ExportTypeKey for the import dependency (HAS TO COME FROM SOME WHERE AS AN EXPORT)
            //

            ExportKey result;

            // IMPORT: Check for the IocImport attribute
            if (dependency.GetCustomAttribute<IocImportAttribute>() != null)
            {
                var importAttribute = dependency.GetCustomAttribute<IocImportAttribute>();

                result = LocateImport(exportTypeKeys, importAttribute.ExportType, importAttribute.ExportKey);
            }

            // NON-IMPORT (EXPORT): Check the assembly's export types
            else
            {
                result = LocateExport(exportTypeKeys, dependency.ParameterType);
            }

            if (result == null)
                throw new IocFailedDependencyException(dependency.ParameterType, "Failed to locate dependency of type {0} as an import or export");

            return result;
        }

        /// <summary>
        /// (LOCATES DEPENDENCY) Searches the assembly's export type keys for the export parameters
        /// </summary>
        private static ExportKey LocateExport(IEnumerable<ExportKey> exportTypeKeys,
                                                  Type exportedType)
        {
            var possibleExports = exportTypeKeys.Where(x => x.ExportedType == exportedType).Actualize();

            if (possibleExports.Count() > 1)
                throw new IocDuplicateExportException(possibleExports, "Duplicate exports found for type {0}", exportedType);

            else if (!possibleExports.Any())
                throw new IocFailedDependencyException(exportedType, "Failed to locate dependency of type {0}", exportedType);

            return possibleExports.First();
        }

        /// <summary>
        /// (LOCATES DEPENDENCY) Searches the assembly's export type keys for the IMPORT based on only the parameters utilized by the 
        /// import attribute
        /// </summary>
        private static ExportKey LocateImport(IEnumerable<ExportKey> exportTypeKeys,
                                                  Type exportedType,
                                                  int exportKey)
        {
            // NOTE*** EXPORT MUST BE KEYED FOR THIS IMPORT TO RESOLVE
            //
            var possibleExports = exportTypeKeys.Where(x => x.ExportedType == exportedType &&
                                                            x.Key == exportKey &&
                                                            x.IsKeyed == true)
                                                .Actualize();

            if (possibleExports.Count() > 1)
                throw new IocDuplicateExportException(possibleExports, "(IMPORT FAILED FOR) Duplicate exports found for type {0}", exportedType);

            else if (!possibleExports.Any())
                throw new IocFailedDependencyException(exportedType, "(IMPORT FAILED FOR) Failed to locate dependency of type {0}");

            return possibleExports.First();
        }
    }
}
