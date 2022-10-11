using Confluent.Kafka;

namespace GGroupp.Infra.Kafka;

public sealed record class KafkaOptions
{
    public KafkaOptions(
        string bootstrapServers, 
        string groupId, 
        string topic,
        AutoOffsetReset autoOffsetReset = AutoOffsetReset.Earliest, 
        bool enableAutoCommit = false, 
        bool enableAutoOffsetStore = true)
    {
        BootstrapServers = bootstrapServers ?? string.Empty;
        GroupId = groupId ?? string.Empty;
        Topic = topic ?? string.Empty;
        AutoOffsetReset = autoOffsetReset;
        EnableAutoCommit = enableAutoCommit;
        EnableAutoOffsetStore = enableAutoOffsetStore;
    }

    internal string BootstrapServers { get; }
 
    internal string GroupId { get; }
    
    internal string Topic { get; }
    
    internal AutoOffsetReset AutoOffsetReset { get; }
    
    internal bool EnableAutoCommit { get; }
    
    internal bool EnableAutoOffsetStore { get; }
}