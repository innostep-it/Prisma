using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prisma.Metrics.Configuration;
using Prisma.Metrics.Services;

namespace Prisma.Metrics.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddMetricsPusher(this IServiceCollection services)
    {
        services.AddSingleton<MetricPusherService>();
        
        services.AddSingleton<IHostedService>(serviceProvider =>
        {
            var config = serviceProvider.GetRequiredService<IOptions<MetricsPusherConfiguration>>();
            var logger = serviceProvider.GetRequiredService<ILogger<MetricPusherService>>();

            return new MetricPusherService(logger, config);
        });
    }
}