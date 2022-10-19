using Demo.Events;
using Devpool.Kafka;
using Microsoft.Extensions.Logging;

namespace Demo.Consumer.EventHandlers;

public class Test1EventHandler:IEventHandler<Test1Event>
{
    private readonly   ILogger<Test1EventHandler>  _logger;
    private readonly IProducer _producer;

    public Test1EventHandler(
        ILogger<Test1EventHandler> logger, 
        IProducer producer)
    {
        _logger = logger;
        _producer = producer;
    }
    
    public async Task HandleAsync(Test1Event @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation(@event.Message);
        await _producer.ProduceAsync(new Test2Event("Send from TestEvent1EventHandler"), cancellationToken);
    }
}