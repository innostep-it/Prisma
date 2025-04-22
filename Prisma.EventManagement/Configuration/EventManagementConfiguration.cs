namespace Prisma.EventManagement.Configuration;

public class EventManagementConfiguration
{
    public string ServiceBusConnectionString { get; set; }
    public ReceivingEventsConfiguration ReceivingEventsConfiguration { get; set; }
    public SendingEventsConfiguration SendingEventsConfiguration { get; set; }
}