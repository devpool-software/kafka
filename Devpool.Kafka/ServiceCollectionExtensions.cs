using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Devpool.Kafka;

public static class ServiceCollectionExtensions
{
    public static void AddKafka(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        services.Configure<KafkaOption>(configuration.GetSection("Kafka"));
        AddProducerAndContext(services);
    }

    public static void AddKafka(this IServiceCollection services, Action<KafkaOption> action)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
        services.Configure<KafkaOption>(configuration.GetSection("Kafka"));
        services.AddOptions<KafkaOption>().PostConfigure(action);
        
        var options = services
            .BuildServiceProvider()
            .GetRequiredService<IOptions<KafkaOption>>()
            .Value;

        var eventHandlerTypes = GetEventHandlerTypes();

        foreach (var eventType in options.EventTypes
                     .Where(eventType => eventHandlerTypes
                         .All(x => x.GetInterfaces().First().GenericTypeArguments.First() != eventType.Type)))
        {
            throw new Exception($"Not handler for event {eventType.Type}");
        }
        
        foreach (var eventHandlerType  in eventHandlerTypes)
             services.AddScoped(eventHandlerType.GetInterfaces().First(), eventHandlerType);

        foreach (var consumer in options.EventTypes.Select(eventType => typeof(Consumer<>).MakeGenericType(eventType.Type)))
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), consumer));
        }

        AddProducerAndContext(services);
    }

    private static void AddProducerAndContext(IServiceCollection services)
    {
        services.AddScoped<IProducer, Producer>();   
        services.AddScoped<KafkaContext>();
    }

    private static List<Type> GetEventHandlerTypes()
    {
        return Assembly
            .GetEntryAssembly()?
            .GetTypes()
            .Where(type => type
                .GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
            .ToList() ?? new List<Type>();
    }
}