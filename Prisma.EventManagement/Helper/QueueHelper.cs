using Prisma.EventManagement.Configuration;

namespace Prisma.EventManagement.Helper;

public static class QueueHelper
{
    public static bool IsQueue<TEvent>(List<ReceivingEventConfiguration> receivingEvents)
    {
        return receivingEvents.First(e => e.EventName == typeof(TEvent).Name).TopicName is null;
    }

    public static string GetQueueName<TEvent>(List<ReceivingEventConfiguration> receivingEvents)
    {
        return receivingEvents.First(e => e.EventName == typeof(TEvent).Name).QueueName!;
    }
}