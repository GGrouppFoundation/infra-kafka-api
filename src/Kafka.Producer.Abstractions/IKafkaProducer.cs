using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Infra.Kafka;

public interface IKafkaProducer<TKey,TValue>
{
    ValueTask<Unit> ProduceAsync(KeyValuePair<TKey,TValue> message, CancellationToken token);
}