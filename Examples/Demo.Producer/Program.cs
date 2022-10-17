using Demo.Events;
using Devpool.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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