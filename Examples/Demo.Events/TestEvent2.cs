using Devpool.Kafka;

namespace Demo.Events;

public record TestEvent2:IEvent
{
    public string Message { get; set; }
}