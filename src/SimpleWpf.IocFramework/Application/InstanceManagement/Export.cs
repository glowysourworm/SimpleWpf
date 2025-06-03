using SimpleWpf.IocFramework.Application.Attribute;
using SimpleWpf.RecursiveSerializer.Shared;

namespace SimpleWpf.IocFramework.Application.InstanceManagement
{
    /// <summary>
    /// Constructs and maintains the reference to the export instance. NOTE** The instance factory information is
    /// not stored here. The ExportCache maintains the ExportKey - which has the instance factory data. Each export
    /// is independent of the other, in that regard. So, if there's an InstanceFactory, it will be resolved upstream.
    /// </summary>
    internal class Export
    {
        /// <summary>
        /// Exported type for the export - this MAY have been set differently to the reflected type
        /// </summary>
        internal Type ExportedType { get; private set; }

        /// <summary>
        /// Reflected type for the export - this should be the actual type constructed
        /// </summary>
        internal Type ReflectedType { get; private set; }

        /// <summary>
        /// Instance policy saved from the export attribute
        /// </summary>
        internal InstancePolicy Policy { get; private set; }

        /// <summary>
        /// The export key saved from the SPECIFIC export attribute
        /// </summary>
        internal int ExportKey { get; private set; }

        /// <summary>
        /// Says whether or not the export was keyed using the SPEIFIC export attribute
        /// </summary>
        internal bool IsExportKeyed { get; private set; }

        /// <summary>
        /// List of dependencies used to create the instance (these are injected to the importing constructor).
        /// NOTE*** THESE ARE PRE-BUILT TO BE INTER-DEPENDENT THE WAY THEY'RE SUPPOSED TO BE IN THE GRAPH. THEY
        ///         ARE ALSO NOT THE INSTANCE FACTORY INJECTED PARAMETERS. If you're using an instance factory, 
        ///         then the both the Export and it's instance factory (also Export) are both resolved independently.
        ///         At runtime, if there is an instance factory, it will be called to create an instance.
        /// </summary>
        internal IEnumerable<Export> Dependencies { get; private set; }

        internal Export(ExportKey exportKey, IEnumerable<Export> dependencies)
        {
            this.ReflectedType = exportKey.ReflectedType;
            this.Dependencies = dependencies;
            this.ExportedType = exportKey.ExportedType;             // Default export type is the reflected type
            this.ExportKey = exportKey.Key;
            this.IsExportKeyed = exportKey.IsKeyed;
            this.Policy = exportKey.Policy;
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            // Create a base hash code by using the recursive serializer to gather value types from the System.Type
            return RecursiveSerializerHashGenerator.CreateSimpleHash(this.ExportedType,
                                                                     this.ReflectedType,
                                                                     this.Policy,
                                                                     this.IsExportKeyed,
                                                                     this.IsExportKeyed ? this.ExportKey : 0);
        }
    }
}
