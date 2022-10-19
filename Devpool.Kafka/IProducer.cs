namespace Devpool.Kafka;

public interface IProducer
{
    Task ProduceAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent;
    Task ProduceRangeAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken) where TEvent : IEvent;
    Task ProduceAsync<TEvent>(TEvent @event) where TEvent : IEvent;
    Task ProduceRangeAsync<TEvent>(IEnumerable<TEvent> events) where TEvent : IEvent;
}