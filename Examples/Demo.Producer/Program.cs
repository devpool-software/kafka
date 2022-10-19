using Demo.Events;
using Devpool.Kafka;
using Microsoft.Extensions.Configuration;
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
await producer.ProduceAsync(new Test2Event("Test message"));

// var list = new List<Test1Event>();
// for (var i = 0; i < 2; i++)
// {
//     list.Add(new Test1Event(i.ToString()));
// }
//
// //Send many events
// await producer.ProduceRangeAsync(list);