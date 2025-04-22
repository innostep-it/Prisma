using Prisma.EventManagement.Configuration;
using Prisma.EventManagement.Services.EventReceivingService;
using Prisma.EventManagement.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Prisma.EventManagement.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddEventHandling(this IServiceCollection services)
    {
        var eventHandlerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
                .ToList());

        foreach (var handlerType in eventHandlerTypes)
        {
            var eventType = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                .GetGenericArguments()[0];

            services.AddScoped(typeof(IEventHandler<>).MakeGenericType(eventType), handlerType);
            
            services.AddSingleton(serviceProvider =>
            {
                var config = serviceProvider.GetRequiredService<IOptions<EventManagementConfiguration>>();
                var logger = serviceProvider.GetRequiredService(
                    typeof(ILogger<>).MakeGenericType(typeof(EventReceivingService<>).MakeGenericType(eventType)));

                return (IHostedService)Activator.CreateInstance(typeof(EventReceivingService<>).MakeGenericType(eventType), logger, config, serviceProvider)!;
            });
        }
    }
}