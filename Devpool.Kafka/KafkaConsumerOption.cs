namespace Devpool.Kafka;

public class KafkaConsumerOption
{
   public string GroupId { get; set; }
   public string BootstrapServers { get; set; }
   public string SaslUsername { get; set; }
   public string SaslPassword { get; set; }
}