using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

namespace GGroupp.Infra.Kafka;

public static class KafkaProducerApiDependency
{
    public static Dependency<IAsyncValueFunc<KeyValuePair<TKey,TValue>, Unit>> UseKafkaProducer<TKey, TValue>(
        this Dependency<ProducerKafkaOptions> dependency)
    {
        _ = dependency ?? throw new ArgumentNullException(nameof(dependency));

        return dependency
            .With(
                new ObjectSerializer<TValue>())
            .Fold<IAsyncValueFunc<KeyValuePair<TKey,TValue>, Unit>>(
                KafkaProducerApi<TKey,TValue>.Create);
    }

    public static IServiceCollection AddKafkaProducer<TKey, TValue>(this IServiceCollection serviceCollection)
        =>
        serviceCollection
            .AddSingleton<IAsyncValueFunc<KeyValuePair<TKey,TValue>, Unit>, KafkaProducerApi<TKey, TValue>>(
                GetKafkaProducerApi<TKey,TValue>);

    private static KafkaProducerApi<TKey, TValue> GetKafkaProducerApi<TKey, TValue>(
        IServiceProvider serviceProvider)
        => 
        KafkaProducerApi<TKey, TValue>.Create(
            serviceProvider.GetKafkaOptions(), 
            new ObjectSerializer<TValue>());

    private static ProducerKafkaOptions GetKafkaOptions(this IServiceProvider serviceProvider)
        => 
        serviceProvider
            .GetRequiredService<IConfiguration>()
            .GetRequiredSection("KafkaOptions")
            .GetKafkaOptions();
    
    private static ProducerKafkaOptions GetKafkaOptions(this IConfigurationSection kafkaSection)
        =>
        new(
            bootstrapServers: kafkaSection["BootstrapServers"], 
            topic: kafkaSection["Topic"]);
}