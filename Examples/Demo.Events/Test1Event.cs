using Devpool.Kafka;

namespace Demo.Events;

public record Test1Event:IEvent
{
    public Test1Event(string message)
    {
        Message = message;
    }

    public string Message { get; set; }
}