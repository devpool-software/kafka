using Devpool.Kafka;

namespace Demo.Events;

public record TestEvent1:IEvent
{
    public TestEvent1(string message)
    {
        Message = message;
    }

    public string Message { get; set; }
}