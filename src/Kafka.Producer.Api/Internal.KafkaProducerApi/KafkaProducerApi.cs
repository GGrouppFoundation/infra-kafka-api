using System;
using System.Collections.Generic;
using Confluent.Kafka;

namespace GGroupp.Infra.Kafka;

internal sealed partial class KafkaProducerApi<TKey, TValue> : IAsyncValueFunc<KeyValuePair<TKey,TValue>, Unit>
{
    private readonly Lazy<IProducer<TKey, TValue>> producer;

    private readonly ProducerKafkaOptions producerKafkaOptions;

    internal static KafkaProducerApi<TKey, TValue> Create(
        ProducerKafkaOptions producerKafkaOptions,
        ISerializer<TValue> objectSerializer)
        => 
        new(
            producerKafkaOptions ?? throw new ArgumentNullException(nameof(producerKafkaOptions)),
            objectSerializer ?? throw new ArgumentNullException(nameof(objectSerializer)));

    private KafkaProducerApi(
        ProducerKafkaOptions producerKafkaOptions, 
        ISerializer<TValue> objectSerializer)
    {
        this.producerKafkaOptions = producerKafkaOptions;
        
        producer = new(
            () => new ProducerBuilder<TKey,TValue>(
                new ProducerConfig
                {
                    BootstrapServers = producerKafkaOptions.BootstrapServers
                })
            .SetValueSerializer(objectSerializer)
            .Build());
    }
}