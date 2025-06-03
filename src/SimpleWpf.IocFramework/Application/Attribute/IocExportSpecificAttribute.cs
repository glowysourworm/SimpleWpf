namespace SimpleWpf.IocFramework.Application.Attribute
{
    /// <summary>
    /// Export attribute for a SPECIFIC instance of an exported class. This will require an
    /// export key. "Specific" exports are treated as SHARED instances keyed by their export key.
    /// </summary>
    // [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class IocExportSpecificAttribute : IocExportBaseAttribute
    {
        /// <summary>
        /// Key to provide as a identifier for the SHARED export.
        /// </summary>
        public int ExportKey { get; private set; }

        /// <summary>
        /// Constructor to specify the export type and instance policy
        /// </summary>
        public IocExportSpecificAttribute(Type exportType, int exportKey, InstancePolicy instancePolicy)
            : base(exportType, null, 0, false, instancePolicy)
        {
            this.ExportKey = exportKey;
        }

        /// <summary>
        /// Constructor to specify the export type and instance policy. Also, export type factory must inherit from IocInstanceFactory.
        /// </summary>
        public IocExportSpecificAttribute(Type exportType, Type exportTypeFactory, int exportTypeFactoryKey, int exportKey, InstancePolicy instancePolicy)
            : base(exportType, exportTypeFactory, exportTypeFactoryKey, true, instancePolicy)
        {
            this.ExportKey = exportKey;
        }
    }
}
