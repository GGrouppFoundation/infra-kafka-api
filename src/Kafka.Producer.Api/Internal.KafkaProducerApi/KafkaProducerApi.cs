using System;
using Confluent.Kafka;

namespace GGroupp.Infra.Kafka;

internal sealed partial class KafkaProducerApi<TKey, TValue, TSerializer> : IKafkaProducer<TKey, TValue>
    where TSerializer : ISerializer<TValue>, IDeserializer<TValue>
{
    private readonly Lazy<IProducer<TKey, TValue>> producer;

    private readonly ProducerKafkaOptions producerKafkaOptions;

    internal static KafkaProducerApi<TKey, TValue, TSerializer> Create(
        ProducerKafkaOptions producerKafkaOptions,
        TSerializer objectSerializer)
        => 
        new(
            producerKafkaOptions ?? throw new ArgumentNullException(nameof(producerKafkaOptions)),
            objectSerializer ?? throw new ArgumentNullException(nameof(objectSerializer)));

    private KafkaProducerApi(
        ProducerKafkaOptions producerKafkaOptions, 
        TSerializer objectSerializer)
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