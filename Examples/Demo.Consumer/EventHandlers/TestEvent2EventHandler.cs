using Demo.Events;
using Devpool.Kafka;
using Microsoft.Extensions.Logging;

namespace Demo.Consumer.EventHandlers;

public class TestEvent2EventHandler:IEventHandler<TestEvent2>
{
    private readonly ILogger _logger;

    public TestEvent2EventHandler(ILogger logger)
    {
        _logger = logger;
    }
    
    public Task HandleAsync(TestEvent2 @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation(@event.Message);
        return Task.CompletedTask;
    }
}