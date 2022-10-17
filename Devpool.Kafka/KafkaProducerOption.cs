namespace Devpool.Kafka;

public class KafkaProducerOption
{
    public string ClientId { get; set; }
    public string BootstrapServers { get; set; }
    public string SaslUsername { get; set; }
    public string SaslPassword { get; set; }
}