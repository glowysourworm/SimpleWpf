namespace SimpleWpf.IocFramework.Application.Attribute
{
    /// <summary>
    /// Primary class for the Ioc Exports. This will cover most cases except for keyed export. For these,
    /// see the IocExportSpecific attribute class. The default instance policy is ShareGlobal
    /// </summary>
    // [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class IocExportAttribute : IocExportBaseAttribute
    {
        /// <summary>
        /// Constructor that sets the instance policy to SHARED
        /// </summary>
        public IocExportAttribute(Type exportType) : base(exportType, null, 0, false, InstancePolicy.ShareGlobal)
        {
        }

        /// <summary>
        /// Constructor for explicit settings for the export. (For the export key setting please use the IocExportSpecific attribute)
        /// </summary>
        public IocExportAttribute(Type exportType, InstancePolicy instancePolicy) : base(exportType, null, 0, false, instancePolicy)
        {
        }

        /// <summary>
        /// Constructor for explicit settings for the export. (For the export key setting please use the IocExportSpecific attribute). 
        /// The export factory type must inherit from IocExportFactory.
        /// </summary>
        public IocExportAttribute(Type exportType, Type exportFactoryType, InstancePolicy instancePolicy)
            : base(exportType, exportFactoryType, 0, false, instancePolicy)
        {
        }
    }
}
