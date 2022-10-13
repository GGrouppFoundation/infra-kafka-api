using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;

namespace GGroupp.Infra.Kafka;

internal sealed class ObjectDeserializer<T> : IDeserializer<T>
{
    private static readonly JsonSerializerOptions jsonSerializerOptions;

    static ObjectDeserializer()
        =>
        jsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

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
