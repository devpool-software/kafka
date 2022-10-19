using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Devpool.Kafka;

public class Consumer<TEvent> : BackgroundService where TEvent: IEvent
{
    private readonly ILogger<Consumer<TEvent>> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ConsumerConfig _config;
    private readonly int _threadCount = 0;

    public Consumer(
        ILogger<Consumer<TEvent>> logger, 
        IOptions<KafkaOption> options,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _config = options.Value.ConsumerConfig;
        _threadCount = options.Value.EventTypes.Single(x => x.Type == typeof(TEvent)).ThreadCount;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = new List<Task>();

       for (var i = 0; i < _threadCount; i++)
       {
           var i1 = i+1;
           tasks.Add(Task.Run(() => ConsumerLoop(i1.ToString(),stoppingToken), stoppingToken));
       }
       
       await Task.WhenAll(tasks);
    }
    
    private async Task ConsumerLoop(string number,CancellationToken stoppingToken)
    {
        try
        {
            
            using var consumer = new ConsumerBuilder<Ignore, string>(_config)
                .SetErrorHandler(ErrorHandler)
                .SetLogHandler(LogHandler)
                .Build();
            
            consumer.Subscribe(GetTopicName());
            _logger.LogInformation($"Consumer<{typeof(TEvent).Name}>={number} subscribe on {GetTopicName()}");
            
            try
            {
                while (true)
                {
                    var result = consumer.Consume(stoppingToken);
                    var correlationId = Encoding.UTF8.GetString(result.Message.Headers.First(x=>x.Key == "CorrelationId").GetValueBytes());
                    _logger.LogInformation($"Consumer<{typeof(TEvent).Name}>={number} start consume event, CorrelationId={correlationId}, Message={result.Message.Value}");
                    var @event = JsonSerializer.Deserialize<TEvent>(result.Message.Value);
                    var handlerType = typeof(IEventHandler<>).MakeGenericType(typeof(TEvent));
                    await using var scope = _serviceProvider.CreateAsyncScope();
                     var context = scope.ServiceProvider.GetRequiredService<KafkaContext>();
                     context.CorrelationId = correlationId;
                     dynamic handler = scope.ServiceProvider.GetRequiredService(handlerType);
                    await handler.HandleAsync((dynamic)@event!, stoppingToken);
                    _logger.LogInformation($"Consumer<{typeof(TEvent).Name}>={number} finish consume event");
                }
            }
            catch (Exception)
            {
                consumer.Close();
                throw;
            }
            
            
        }
        catch (Exception ex)
        {
            _logger.LogError($"Consumer<{typeof(TEvent).Name}> Error={ex.Message}");
            throw;
        }
    }
    
    private string GetTopicName()
    {
        var value = typeof(TEvent)
            .ToString()
            .Split(".")
            .Last();

        return Regex.Replace(value, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", "-$1", RegexOptions.Compiled)
            .Trim()
            .ToLower();
    }

    private  void LogHandler(IConsumer<Ignore, string> consumer, LogMessage message)
    {
        _logger.LogInformation($"Consumer<{typeof(TEvent).Name}> Message="+message.Message);
    }

    private  void ErrorHandler(IConsumer<Ignore, string> consumer, Error error)
    {
        _logger.LogError($"Consumer<{typeof(TEvent).Name}> Error="+error.Reason);
    }
    
}