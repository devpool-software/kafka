using Devpool.Kafka;

namespace Demo.Events;

public record Test2Event:IEvent
{
    public Test2Event(string message)
    {
        Message = message;
    }

    public string Message { get; set; }
}