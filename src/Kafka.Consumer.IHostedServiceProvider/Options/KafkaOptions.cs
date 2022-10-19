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

    public string BootstrapServers { get; }
 
    public string GroupId { get; }
    
    public string Topic { get; }
    
    public AutoOffsetReset AutoOffsetReset { get; }
    
    public bool EnableAutoCommit { get; }
    
    public bool EnableAutoOffsetStore { get; }
}