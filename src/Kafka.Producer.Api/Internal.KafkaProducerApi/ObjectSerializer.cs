using System;
using System.Text.Json;
using Confluent.Kafka;

namespace GGroupp.Infra.Kafka;

internal sealed class ObjectSerializer<T> : ISerializer<T>
{
    public byte[] Serialize(T data, SerializationContext context) => JsonSerializer.SerializeToUtf8Bytes(data);
}   
