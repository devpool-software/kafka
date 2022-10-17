using Demo.Events;
using Devpool.Kafka;
using Microsoft.Extensions.Logging;

namespace Demo.Consumer.EventHandlers;

public class TestEvent1EventHandler:IEventHandler<TestEvent1>
{
    private readonly ILogger _logger;

    public TestEvent1EventHandler(ILogger logger)
    {
        _logger = logger;
    }
    
    public Task HandleAsync(TestEvent1 @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation(@event.Message);
        return Task.CompletedTask;
    }
}