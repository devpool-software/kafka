using Confluent.Kafka;

namespace Devpool.Kafka;

public class KafkaOption
{
    public KafkaOption()
    {
        EventTypes = new List<EventTypeOption>();
    }

    public List<EventTypeOption> EventTypes { get; }
    
    public void Subscribe<T>(int threadCount = 1) where T : IEvent
    {
        EventTypes.Add(new EventTypeOption(typeof(T), threadCount));
    }

    public ConsumerConfig ConsumerConfig { get; set; }
    
    public ProducerConfig ProducerConfig { get; set; }
}