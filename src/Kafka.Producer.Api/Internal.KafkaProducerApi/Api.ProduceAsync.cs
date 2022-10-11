using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace GGroupp.Infra.Kafka;

partial class KafkaProducerApi<TKey, TValue, TSerializer>
{
    public async ValueTask<Unit> InvokeAsync(KeyValuePair<TKey, TValue> message, CancellationToken token)
    {
        await producer.Value.ProduceAsync(
            producerKafkaOptions.Topic,
            new Message<TKey, TValue>
            {
                Key = message.Key,
                Value = message.Value
            },
            token)
            .ConfigureAwait(false);
        
        return default;
    }
}