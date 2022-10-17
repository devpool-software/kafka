namespace Devpool.Kafka;

public class EventTypeOption
{
    public EventTypeOption(Type type, int threadCount)
    {
        Type = type;
        ThreadCount = threadCount;
    }

    public Type Type { get; set; }
    public int ThreadCount { get; set; }
}