using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GGroupp.Infra.Kafka;

internal sealed partial class KafkaHostedService<TKey, TValue> : IHostedService
{
    private readonly KafkaOptions kafkaOptions;
    
    private readonly RetryPolicyOptions retryPolicyOptions;
    
    private readonly IDeserializer<TValue> objectSerializer;
    
    private readonly ILogger logger;
    
    private readonly IAsyncValueFunc<ConsumeResult<TKey, TValue>, Unit> messageHandler;

    private Task? backgroundTask;

    private CancellationTokenSource? cancellationTokenSource;

    public static KafkaHostedService<TKey, TValue> Create(
        IAsyncValueFunc<ConsumeResult<TKey, TValue>, Unit> messageHandler,
        KafkaOptions kafkaOptions,
        RetryPolicyOptions retryPolicyOptions,
        IDeserializer<TValue> objectSerializer,
        ILoggerFactory loggerFactory)
        => 
        new(
            kafkaOptions ?? throw new ArgumentNullException(nameof(kafkaOptions)),
            retryPolicyOptions ?? throw new ArgumentNullException(nameof(retryPolicyOptions)),
            objectSerializer ?? throw new ArgumentNullException(nameof(objectSerializer)),
            messageHandler ?? throw new ArgumentNullException(nameof(messageHandler)),
            loggerFactory?.CreateLogger<KafkaHostedService<TKey, TValue>>() ?? throw new ArgumentNullException(nameof(loggerFactory)));
    
    private KafkaHostedService(
        KafkaOptions kafkaOptions, 
        RetryPolicyOptions retryPolicyOptions, 
        IDeserializer<TValue> objectSerializer,
        IAsyncValueFunc<ConsumeResult<TKey, TValue>, Unit> messageHandler,
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