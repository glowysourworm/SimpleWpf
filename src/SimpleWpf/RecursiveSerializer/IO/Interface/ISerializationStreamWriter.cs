using System;
using System.Collections.Generic;

using SimpleWpf.RecursiveSerializer.Manifest;

namespace SimpleWpf.RecursiveSerializer.IO.Interface
{
    internal interface ISerializationStreamWriter
    {
        void Write<T>(T theObject);
        void Write(object theObject, Type theObjectType);

        IEnumerable<SerializedStreamData> GetStreamData();
    }
}
