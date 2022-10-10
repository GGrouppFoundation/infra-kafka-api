using System;
using System.Text.Json;
using Confluent.Kafka;

namespace GGroupp.Infra.Kafka;

internal sealed class ObjectSerializer<T> : ISerializer<T>, IDeserializer<T>
{
    public byte[] Serialize(T data, SerializationContext context) => JsonSerializer.SerializeToUtf8Bytes(data);

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            throw new ArgumentNullException(nameof(data));

        return JsonSerializer.Deserialize<T>(data) ?? throw new ArgumentException(nameof(data));
    }
}   
