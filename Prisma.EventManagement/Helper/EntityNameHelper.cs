using Prisma.EventManagement.Configuration;

namespace Prisma.EventManagement.Helper;

public static class EntityNameHelper
{
    public static string GetEntityName(string eventName, List<SendingEventConfiguration> sendingEvents)
    {
        var senderName = sendingEvents.FirstOrDefault(e => e.EventName == eventName)?.EntityName;
        if (senderName == null)
        {
            throw new Exception($"Sender name not found for event {eventName}");
        }

        return senderName;
    }
}