
using Demo.Events;
using Devpool.Kafka;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddKafka(option =>
        {
            option.Subscribe<Test1Event>(3);
            option.Subscribe<Test2Event>(6);
        });
    })
    .Build();

host.Run();