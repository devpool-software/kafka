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

    public Producer(
        ILogger<Producer> logger,
        IOptions<KafkaOption> options)
    {
        _logger = logger;
        _producerConfig = options.Value.ProducerConfig;
    }
    
    public async Task ProduceAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        try
        {
            using var producer = new ProducerBuilder<string, string>(_producerConfig).Build();
            var topic = GetTopicName(typeof(TEvent));
            var json = JsonSerializer.Serialize(@event);
            _logger.LogInformation($"Send {topic}");
            await producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = json
            });
            producer.Flush(TimeSpan.FromSeconds(10));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
    
    public async Task ProduceRangeAsync<TEvent>(IEnumerable<TEvent> events) where TEvent : IEvent
    {
        try
        {
            using var producer = new ProducerBuilder<string, string>(_producerConfig).Build();
            var topic = GetTopicName(typeof(TEvent));
            foreach (var json in events.Select(@event => JsonSerializer.Serialize(@event)))
            {
                _logger.LogInformation($"Send {topic}");
                await producer.ProduceAsync(topic, new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = json
                });
                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
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