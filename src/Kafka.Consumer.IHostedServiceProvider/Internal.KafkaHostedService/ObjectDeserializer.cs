using System;
using System.Text.Json;
using Confluent.Kafka;

namespace GGroupp.Infra.Kafka;

internal sealed class ObjectDeserializer<T> : IDeserializer<T>
{
    private static readonly JsonSerializerOptions jsonSerializerOptions;

    static ObjectDeserializer()
        =>
        jsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
        {
            throw new InvalidOperationException("Data must be not null");
        }

        return JsonSerializer.Deserialize<T>(data, jsonSerializerOptions) ?? 
               throw new InvalidOperationException("Invalid data type or data is empty");
    }
}   
