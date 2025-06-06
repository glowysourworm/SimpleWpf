﻿
using System;
using System.IO;

using SimpleWpf.Extensions;
using SimpleWpf.RecursiveSerializer.Component;
using SimpleWpf.RecursiveSerializer.IO;
using SimpleWpf.RecursiveSerializer.Manifest;

namespace SimpleWpf.RecursiveSerializer.Shared
{
    /// <summary>
    /// Serializer that performs depth-first serialization / deserialization
    /// </summary>
    public class RecursiveSerializer<T>
    {
        PropertySerializer _serializer;
        PropertyDeserializer _deserializer;

        public RecursiveSerializer(RecursiveSerializerConfiguration configuration)
        {
            _serializer = new PropertySerializer(configuration);
            _deserializer = new PropertyDeserializer(configuration);
        }

        public RecursiveSerializer(Type objectType, RecursiveSerializerConfiguration configuration)
        {
            // Double check that assembly is loaded and that object will have proper serialization data support
            if (!SimpleWpf.RecursiveSerializer.Shared.RecursiveSerializerTypeFactory.IsAssemblyLoaded(objectType))
                throw new FormattedException("Assembly not loaded for RecursiveSerializer for type {0}", objectType);

            _serializer = new PropertySerializer(configuration);
            _deserializer = new PropertyDeserializer(configuration);
        }

        public void Serialize(Stream stream, T theObject)
        {
            var serializationStream = new SerializationStream(stream);

            _serializer.Serialize(serializationStream, theObject);
        }

        public T Deserialize(Stream stream)
        {
            var serializationStream = new SerializationStream(stream);

            return _deserializer.Deserialize<T>(serializationStream);
        }

        public SerializationManifest CreateDeserializationManifest()
        {
            return new SerializationManifest(_deserializer.GetDeserializedHeader());
        }
    }
}
