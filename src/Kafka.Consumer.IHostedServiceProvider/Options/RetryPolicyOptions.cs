using System;

namespace GGroupp.Infra.Kafka;

public record RetryPolicyOptions
{
    public RetryPolicyOptions(int retryCount, TimeSpan medianFirstRetryDelay)
    {
        RetryCount = retryCount;
        MedianFirstRetryDelay = medianFirstRetryDelay;
    }

    internal int RetryCount { get; }
    
    internal TimeSpan MedianFirstRetryDelay { get; }
}