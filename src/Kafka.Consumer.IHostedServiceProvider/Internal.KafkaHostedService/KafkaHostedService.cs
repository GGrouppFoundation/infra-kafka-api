using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GGroupp.Infra.Kafka;

internal sealed partial class KafkaHostedService<TKey, TValue, TSerializer> : IHostedService
    where TSerializer : ISerializer<TValue>, IDeserializer<TValue>
{
    private readonly KafkaOptions kafkaOptions;
    
    private readonly RetryPolicyOptions retryPolicyOptions;
    
    private readonly TSerializer objectSerializer;
    
    private readonly ILogger logger;
    
    private readonly Func<ConsumeResult<TKey, TValue>, CancellationToken, ValueTask> messageHandler;

    private Task? backgroundTask;

    private CancellationTokenSource? cancellationTokenSource;

    public static KafkaHostedService<TKey, TValue, TSerializer> Create(
        KafkaOptions kafkaOptions,
        RetryPolicyOptions retryPolicyOptions,
        Func<ConsumeResult<TKey, TValue>, CancellationToken, ValueTask> messageHandler,
        TSerializer objectSerializer,
        ILogger logger)
        => 
        new(
            kafkaOptions ?? throw new ArgumentNullException(nameof(kafkaOptions)),
            retryPolicyOptions ?? throw new ArgumentNullException(nameof(retryPolicyOptions)),
            objectSerializer ?? throw new ArgumentNullException(nameof(objectSerializer)),
            messageHandler ?? throw new ArgumentNullException(nameof(messageHandler)),
            logger ?? throw new ArgumentNullException(nameof(logger)));
    
    private KafkaHostedService(
        KafkaOptions kafkaOptions, 
        RetryPolicyOptions retryPolicyOptions, 
        TSerializer objectSerializer,
        Func<ConsumeResult<TKey, TValue>, CancellationToken, ValueTask> messageHandler,
        ILogger logger)
    {
        this.kafkaOptions = kafkaOptions;
        this.retryPolicyOptions = retryPolicyOptions;
        this.objectSerializer = objectSerializer;
        this.messageHandler = messageHandler;
        this.logger = logger;

        backgroundTask = null;
        cancellationTokenSource = null;
    }
}