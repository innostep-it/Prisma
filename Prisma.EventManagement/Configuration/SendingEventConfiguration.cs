namespace Prisma.EventManagement.Configuration;

public class SendingEventsConfiguration
{
    public List<SendingEventConfiguration> SendingEvents { get; set; } 
}

public class SendingEventConfiguration
{
    public string EventName { get; set; }
    public string EntityName { get; set; }
}