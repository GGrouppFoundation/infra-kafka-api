using System;
using System.Text.Json;
using Confluent.Kafka;

namespace GGroupp.Infra.Kafka;

internal sealed class ObjectDeserializer<T> : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            throw new InvalidOperationException(nameof(data));

        return JsonSerializer.Deserialize<T>(data) ?? throw new InvalidOperationException(nameof(data));
    }
}   
