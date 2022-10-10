using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

namespace GGroupp.Infra.Kafka;

public static class KafkaApiDependency
{
    public static Dependency<IHostedService> UseKafkaHostedService<TKey, TValue>(
        this Dependency<KafkaOptions, RetryPolicyOptions,
            Func<ConsumeResult<TKey, TValue>, CancellationToken, ValueTask>> dependency)
    {
        _ = dependency ?? throw new ArgumentNullException(nameof(dependency));

        return dependency.With(new ObjectSerializer<TValue>())
            .With(
                sp => sp.GetRequiredService<ILoggerFactory>()
                    .CreateLogger<KafkaHostedService<TKey, TValue, ObjectSerializer<TValue>>>())
            .Fold<IHostedService>(KafkaHostedService<TKey, TValue, ObjectSerializer<TValue>>.Create);
    }

    public static void AddKafkaHostedService<TKey,TValue>(
        this IServiceCollection serviceCollection,
        KafkaOptions kafkaOptions,
        RetryPolicyOptions retryPolicyOptions,
        Func<ConsumeResult<TKey, TValue>, CancellationToken, ValueTask> messageHandler)
    {
        serviceCollection
            .AddHostedService(
                sp => KafkaHostedService<TKey, TValue, ObjectSerializer<TValue>>.Create(
                    kafkaOptions: kafkaOptions,
                    retryPolicyOptions: retryPolicyOptions,
                    messageHandler: messageHandler,
                    objectSerializer: new ObjectSerializer<TValue>(),
                    logger: sp.GetLogger<TKey, TValue>()));

    }
    
    public static IServiceCollection AddKafkaHostedService<TKey,TValue>(
        this IServiceCollection serviceCollection,
        Func<ConsumeResult<TKey, TValue>, CancellationToken, ValueTask> messageHandler)
        =>
        serviceCollection
            .AddHostedService<KafkaHostedService<TKey, TValue, ObjectSerializer<TValue>>>(
                sp => KafkaHostedService<TKey, TValue, ObjectSerializer<TValue>>.Create(
                    kafkaOptions: sp.GetKafkaOptions(),
                    retryPolicyOptions: sp.GetRetryPolicyOptions(),
                    messageHandler: messageHandler,
                    objectSerializer: new ObjectSerializer<TValue>(),
                    logger: sp.GetLogger<TKey, TValue>()));

    private static KafkaOptions GetKafkaOptions(this IServiceProvider serviceProvider)
        =>
        serviceProvider
            .GetRequiredService<IConfiguration>()
            .GetRequiredSection("KafkaOptions")
            .GetKafkaOptions();

    private static KafkaOptions GetKafkaOptions(this IConfigurationSection kafkaSection)
        =>
        new(
            bootstrapServers: kafkaSection["BootstrapServers"],
            groupId: kafkaSection["GroupId"],
            topic: kafkaSection["Topic"],
            autoOffsetReset: kafkaSection.GetValue<AutoOffsetReset>("AutoOffsetReset"),
            enableAutoCommit: kafkaSection.GetValue<bool>("EnableAutoCommit"),
            enableAutoOffsetStore: kafkaSection.GetValue<bool>("EnableAutoOffsetStore"));
    
    private static RetryPolicyOptions GetRetryPolicyOptions(this IServiceProvider serviceProvider)
        =>
        serviceProvider
            .GetRequiredService<IConfiguration>()
            .GetRequiredSection("RetryPolicyOptions")
            .GetRetryPolicyOptions();

    private static RetryPolicyOptions GetRetryPolicyOptions(this IConfigurationSection retryPolicySection)
        =>
        new(
            retryCount: retryPolicySection.GetValue<int>("RetryCount"),
            medianFirstRetryDelay: retryPolicySection.GetValue<TimeSpan>("MedianFirstRetryDelay"));
    
    private static ILogger GetLogger<TKey,TValue>(this IServiceProvider serviceProvider)
        => 
        serviceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger<KafkaHostedService<TKey, TValue, ObjectSerializer<TValue>>>();
}