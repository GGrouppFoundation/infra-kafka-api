using System;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PrimeFuncPack;

namespace GGroupp.Infra.Kafka;

public static class KafkaConsumerServiceDependency
{
    public static Dependency<IHostedService> UseKafkaHostedService<TKey, TValue>(
        this Dependency<IAsyncValueFunc<ConsumeResult<TKey, TValue>, Unit>, KafkaOptions, RetryPolicyOptions> dependency)
    {
        _ = dependency ?? throw new ArgumentNullException(nameof(dependency));

        return dependency
            .With(
                new ObjectDeserializer<TValue>())
            .With(
                sp => sp.GetRequiredService<ILoggerFactory>())
            .Fold<IHostedService>(
                KafkaHostedService<TKey, TValue>.Create);
    }

    public static IServiceCollection AddKafkaHostedService<TKey,TValue>(
        this IServiceCollection serviceCollection,
        IAsyncValueFunc<ConsumeResult<TKey, TValue>, Unit> messageHandler,
        KafkaOptions kafkaOptions,
        RetryPolicyOptions retryPolicyOptions)
        =>
        serviceCollection
            .AddHostedService(
                sp => KafkaHostedService<TKey, TValue>.Create(
                    kafkaOptions: kafkaOptions,
                    retryPolicyOptions: retryPolicyOptions,
                    messageHandler: messageHandler,
                    objectSerializer: new ObjectDeserializer<TValue>(),
                    loggerFactory: sp.GetRequiredService<ILoggerFactory>()));
    
    public static IServiceCollection AddKafkaHostedService<TKey,TValue>(
        this IServiceCollection serviceCollection,
        IAsyncValueFunc<ConsumeResult<TKey, TValue>, Unit>messageHandler)
        =>
        serviceCollection
            .AddHostedService(
                sp => KafkaHostedService<TKey, TValue>.Create(
                    kafkaOptions: sp.GetKafkaOptions(),
                    retryPolicyOptions: sp.GetRetryPolicyOptions(),
                    messageHandler: messageHandler,
                    objectSerializer: new ObjectDeserializer<TValue>(),
                    loggerFactory: sp.GetRequiredService<ILoggerFactory>()));

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
}