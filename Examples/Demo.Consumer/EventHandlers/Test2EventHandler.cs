using Demo.Events;
using Devpool.Kafka;
using Microsoft.Extensions.Logging;

namespace Demo.Consumer.EventHandlers;

public class Test2EventHandler:IEventHandler<Test2Event>
{
    private readonly   ILogger<Test2EventHandler>  _logger;
    private readonly IProducer _producer;
    
    public Test2EventHandler(
        ILogger<Test2EventHandler> logger, 
        IProducer producer)
    {
        _logger = logger;
        _producer = producer;
    }
    
    public async Task HandleAsync(Test2Event @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation(@event.Message);
        await _producer.ProduceAsync(new Test1Event("Send from TestEvent1EventHandler"), cancellationToken);
    }
}