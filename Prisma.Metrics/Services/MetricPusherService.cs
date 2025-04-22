using System.Net.Http.Headers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prisma.Metrics.Configuration;
using Prometheus;

namespace Prisma.Metrics.Services;

public class MetricPusherService(ILogger<MetricPusherService> logger, IOptions<MetricsPusherConfiguration> config): BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {        
        logger.LogInformation("MetricPusherService Start ExecuteAsync");

        var httpClient = new HttpClient();
        var authValue = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"{config.Value.Username}:{config.Value.Password}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);
        
        var metricPusher = new MetricPusher(new MetricPusherOptions
        {
            Endpoint = config.Value.Endpoint,
            Job = config.Value.JobName,
            Instance = Guid.NewGuid().ToString(),
            HttpClientProvider = () => httpClient,
            IntervalMilliseconds = 5000
        });
        
        metricPusher.Start();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            // todo: Hier können Sie Ihre Metriken sammeln und an den Pusher senden
            
            await Task.Delay(config.Value.PushCustomMetricsIntervalInSeconds, stoppingToken);
        }

        await metricPusher.StopAsync();
    }
}