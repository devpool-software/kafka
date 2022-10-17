
using Demo.Events;
using Devpool.Kafka;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddKafka(option =>
        {
            option.Subscribe<TestEvent1>(2);
            option.Subscribe<TestEvent2>(3);
        });
    })
    .Build();

host.Run();