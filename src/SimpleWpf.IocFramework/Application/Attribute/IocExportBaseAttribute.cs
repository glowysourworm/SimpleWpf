namespace SimpleWpf.IocFramework.Application.Attribute
{
    /// <summary>
    /// Abstract base class for the Ioc Exports. THIS DEFAULTS THE INSTANCE POLICY TO SHARED
    /// </summary>
    // [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class IocExportBaseAttribute : System.Attribute
    {
        /// <summary>
        /// Returns the export type - which is null for the attribute until it is resolved by the 
        /// export factory.
        /// </summary>
        internal Type? ExportType { get; private set; }

        /// <summary>
        /// Defines the export factory for this type of component. 
        /// This factory must inherit from IocInstanceFactory.
        /// </summary>
        public Type? ExportFactoryType { get; private set; }

        /// <summary>
        /// Specifies export factory instance (if applicable)
        /// </summary>
        public int ExportFactoryInstanceId { get; private set; }

        /// <summary>
        /// Specifies that export factroy has a specific instance. Must be consistent with export
        /// factory attributes
        /// </summary>
        public bool ExportFactoryInstanceSpecific { get; private set; }

        /// <summary>
        /// Returns the instance policy for the export. This is shared by default.
        /// </summary>
        internal InstancePolicy InstancePolicy { get; private set; }

        /// <summary>
        /// Creates an export that is keyed by the supplied key. This key must be supplied when re-calling the instance.
        /// </summary>
        public IocExportBaseAttribute(Type? exportType, Type? exportFactoryType, int exportFactoryInstanceId, bool exportFactoryIsSpecificInstance, InstancePolicy instancePolicy)
        {
            this.ExportType = exportType;
            this.ExportFactoryType = exportFactoryType;
            this.ExportFactoryInstanceId = exportFactoryInstanceId;
            this.ExportFactoryInstanceSpecific = exportFactoryIsSpecificInstance;
            this.InstancePolicy = instancePolicy;
        }
    }
}
