// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prisma.EventManagement;
using Prisma.EventManagement.Configuration;
using Prisma.EventManagement.Services.RabbitMq;
using Prisma.EventManagement.Tests;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Konfiguration registrieren
        services.Configure<EventManagementConfiguration>(context.Configuration.GetSection("EventManagement"));

        // Services registrieren
        services.AddSingleton<RabbitMqEventPublicationService>();
        services.AddSingleton(typeof(RabbitMqEventReceivingService<>));
        services.AddSingleton<RabbitMqServiceManager>();

        // Logger registrieren
        services.AddLogging(configure => configure.AddConsole());
    })
    .Build();

var serviceManager = host.Services.GetRequiredService<RabbitMqServiceManager>();
await serviceManager.RunAsync();