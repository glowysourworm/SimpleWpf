using System.IO;

namespace SimpleWpf.RecursiveSerializer.Interface
{
    /// <summary>
    /// Convenience interface for BaseFormatter implementations
    /// </summary>
    internal interface IBaseFormatter
    {
        object Read(Stream stream);

        void Write(Stream stream, object theObject);
    }
}
