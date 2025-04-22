namespace Prisma.Metrics.Configuration;

public class MetricsPusherConfiguration
{
    public string Endpoint { get; set; }
    public string JobName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int PushCustomMetricsIntervalInSeconds { get; set; } = 10;
}