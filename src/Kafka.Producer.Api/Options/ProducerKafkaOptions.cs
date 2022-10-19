namespace GGroupp.Infra.Kafka;

public sealed record class ProducerKafkaOptions
{
    public ProducerKafkaOptions(
        string bootstrapServers, 
        string topic)
    {
        BootstrapServers = bootstrapServers ?? string.Empty;
        Topic = topic ?? string.Empty;
    }

    public string BootstrapServers { get; }
 
    public string Topic { get; }
}