using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeFuncPack;

namespace GGroupp.Infra.Kafka;

public static class KafkaProducerApiDependency
{
    public static Dependency<IKafkaProducer<TKey, TValue>> UseKafkaProducer<TKey, TValue>(
        this Dependency<ProducerKafkaOptions> dependency)
    {
        _ = dependency ?? throw new ArgumentNullException(nameof(dependency));

        return dependency
            .With(
                new ObjectSerializer<TValue>())
            .Fold<IKafkaProducer<TKey, TValue>>(
                KafkaProducerApi<TKey,TValue,ObjectSerializer<TValue>>.Create);
    }

    public static IServiceCollection AddKafkaProducer<TKey, TValue>(this IServiceCollection serviceCollection)
        =>
        serviceCollection
            .AddSingleton<IKafkaProducer<TKey, TValue>, KafkaProducerApi<TKey, TValue, ObjectSerializer<TValue>>>(
                GetKafkaProducerApi<TKey,TValue>);

    private static KafkaProducerApi<TKey, TValue, ObjectSerializer<TValue>> GetKafkaProducerApi<TKey, TValue>(
        IServiceProvider serviceProvider)
        => 
        KafkaProducerApi<TKey, TValue, ObjectSerializer<TValue>>.Create(
            serviceProvider.GetKafkaOptions(), 
            new());

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