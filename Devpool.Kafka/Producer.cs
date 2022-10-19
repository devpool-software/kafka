using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Devpool.Kafka;

public class Producer:IProducer
{
    private readonly ILogger<Producer> _logger;
    private readonly ProducerConfig _producerConfig;
    private readonly KafkaContext _context;
    public Producer(
        ILogger<Producer> logger,
        IOptions<KafkaOption> options,
        KafkaContext context)
    {
        _logger = logger;
        _context = context;
        _producerConfig = options.Value.ProducerConfig;
        
    }
    
    public async Task ProduceAsync<TEvent>(TEvent @event,CancellationToken cancellationToken) where TEvent : IEvent
    {
        try
        {
            using var producer = new ProducerBuilder<string, string>(_producerConfig).Build();
            var correlationId = _context.CorrelationId ?? Guid.NewGuid().ToString();
            var topic = GetTopicName(typeof(TEvent));
            var json = JsonSerializer.Serialize(@event);
            _logger.LogInformation($"Send topic={topic} correlationId={correlationId} message={json}");
            await producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = json,
                Headers = new Headers
                {
                    new Header("CorrelationId", Encoding.UTF8.GetBytes(correlationId))
                } 
            }, cancellationToken);
            producer.Flush(TimeSpan.FromSeconds(10));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
    
    public async Task ProduceRangeAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken) where TEvent : IEvent
    {
        try
        {
            using var producer = new ProducerBuilder<string, string>(_producerConfig).Build();
            var topic = GetTopicName(typeof(TEvent));
            foreach (var @event in events)
            {
                var json = JsonSerializer.Serialize(@event);
                var correlationId = _context.CorrelationId ?? Guid.NewGuid().ToString();
                _logger.LogInformation($"Send topic={topic} correlationId={correlationId} message={json}");
                await producer.ProduceAsync(topic, new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = json,
                    Headers = new Headers
                    {
                        new Header("CorrelationId", Encoding.UTF8.GetBytes(correlationId))
                    } 
                }, cancellationToken);
                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    public Task ProduceAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        return ProduceAsync(@event, new CancellationToken());
    }

    public Task ProduceRangeAsync<TEvent>(IEnumerable<TEvent> events) where TEvent : IEvent
    {
        return ProduceRangeAsync(events, new CancellationToken());
    }

    private string GetTopicName(Type eventType)
    {
        var value = eventType
            .ToString()
            .Split(".")
            .Last();

        return Regex.Replace(value, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", "-$1", RegexOptions.Compiled)
            .Trim()
            .ToLower();
    }
}