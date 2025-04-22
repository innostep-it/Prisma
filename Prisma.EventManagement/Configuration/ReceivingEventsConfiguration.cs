namespace Prisma.EventManagement.Configuration;

public class ReceivingEventsConfiguration
{
    public List<ReceivingEventConfiguration> ReceivingEvents { get; set; }
}

public class ReceivingEventConfiguration
{
    public string EventName { get; set; }
    public string? TopicName { get; set; }
    public string? TopicSubscription { get; set; }
    public string? QueueName { get; set; }
}