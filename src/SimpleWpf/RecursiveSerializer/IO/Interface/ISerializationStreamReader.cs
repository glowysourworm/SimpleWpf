using System;
using System.Collections.Generic;

using SimpleWpf.RecursiveSerializer.Manifest;

namespace SimpleWpf.RecursiveSerializer.IO.Interface
{
    internal interface ISerializationStreamReader
    {
        T Read<T>();
        object Read(Type type);

        IEnumerable<SerializedStreamData> GetStreamData();
    }
}
