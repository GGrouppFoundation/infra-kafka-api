using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace GGroupp.Infra.Kafka;

partial class KafkaHostedService<TKey, TValue>
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        backgroundTask = ExecuteAsync(cancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    private async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var backOffDelay = Backoff.DecorrelatedJitterBackoffV2(
            retryCount: retryPolicyOptions.RetryCount,
            medianFirstRetryDelay: retryPolicyOptions.MedianFirstRetryDelay);

        await Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                sleepDurations: backOffDelay,
                onRetry: OnRetry)
            .ExecuteAsync(
                action: () => InnerConsumeAsync(stoppingToken))
            .ConfigureAwait(false);
        
        void OnRetry(Exception ex, TimeSpan _) 
            =>
            logger.LogError("Error occured, trying to reconnect {Message}", ex.Message);   
    }

    private async Task InnerConsumeAsync(
        CancellationToken token)
    {
        using var consumer = new ConsumerBuilder<TKey, TValue>(
                new ConsumerConfig
                {
                    BootstrapServers = kafkaOptions.BootstrapServers,
                    GroupId = kafkaOptions.GroupId,
                    AutoOffsetReset = kafkaOptions.AutoOffsetReset,
                    EnableAutoCommit = kafkaOptions.EnableAutoCommit,
                    EnableAutoOffsetStore = kafkaOptions.EnableAutoOffsetStore
                })
            .SetValueDeserializer(objectSerializer)
            .Build();

        consumer.Subscribe(kafkaOptions.Topic);

        await Task.Yield();

        while (consumer.Consume(token) is { } result)
        {
            await messageHandler.InvokeAsync(result, token).ConfigureAwait(false);

            if (kafkaOptions.EnableAutoCommit is false)
            {
                consumer.Commit(result);
            }
        }

        consumer.Close();
    }
}