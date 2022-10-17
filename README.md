Event IEvent
````csharp
public record TestEvent1:IEvent
{
    public TestEvent1(string message)
    {
        Message = message;
    }

    public string Message { get; set; }
}
````

Event handler IEventHandler<T>
````csharp
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
````

Producer
````csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //Include IProducer
        services.AddKafka();
    })
    .Build();
    
    
var producer = host.Services.GetRequiredService<IProducer>();

//Send one event
await producer.ProduceAsync(new TestEvent1("Test message"));

var list = new List<TestEvent1>();
for (var i = 0; i < 1000; i++)
{
    list.Add(new TestEvent1(i.ToString()));
}

//Send many events
await producer.ProduceRangeAsync(list);
````

Subscribe for messages

````csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddKafka(option =>
        {
            option.Subscribe<TestEvent1>(2);
            option.Subscribe<TestEvent2>();
        });
    })
    .Build();

host.Run();
````
```option.Subscribe<TestEvent1>(3);``` 3 thread subscribe one group-id

```option.Subscribe<TestEvent2>();``` default 1 thread subscribe one group-id

Configuration

appsettings.json
```json
{
  "Kafka": {
    "ConsumerConfig": {
      "BootstrapServers": "YOUR_ADDRESS",
      "SaslUsername": "user",
      "SaslPassword": "12345678",
      "GroupId": "consumer-1",
      "AutoOffsetReset" : "Earliest",
      "SaslMechanism" : "ScramSha512",
      "SecurityProtocol" : "SaslPlaintext"
    },
    "ProducerConfig": {
      "BootstrapServers": "YOUR_ADDRESS",
      "SaslUsername": "user",
      "SaslPassword": "1234578",
      "ClientId": "client-1",
      "SaslMechanism" : "ScramSha512",
      "SecurityProtocol" : "SaslPlaintext",
      "EnableDeliveryReports" : true,
      "Acks": "All",
      "MessageSendMaxRetries" : 3,
      "RetryBackoffMs" : 1000,
      "EnableIdempotence" : true
    }
  }
}

```