namespace Devpool.Kafka;

public class KafkaContext
{
    public KafkaContext()
    {
        CorrelationId = null;
    }
    
    public string? CorrelationId { get; set; }
}