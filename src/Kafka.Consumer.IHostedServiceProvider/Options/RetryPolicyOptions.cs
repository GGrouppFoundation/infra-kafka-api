using System;

namespace GGroupp.Infra.Kafka;

public sealed record class RetryPolicyOptions
{
    public RetryPolicyOptions(int retryCount, TimeSpan medianFirstRetryDelay)
    {
        RetryCount = retryCount;
        MedianFirstRetryDelay = medianFirstRetryDelay;
    }

    public int RetryCount { get; }
    
    public TimeSpan MedianFirstRetryDelay { get; }
}