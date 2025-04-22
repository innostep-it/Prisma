using Prisma.EventManagement.Shared;

namespace Prisma.EventManagement.Helper;

public static class BasePropertiesHelper
{
    public static void SetBaseEventProperties<TEntity>(TEntity eventData, DateTimeOffset enqueuedTime) where TEntity : BaseEvent
    {
        eventData.EnqueuedTime = enqueuedTime;
        eventData.CorrelationId = "Prisma.EventManagement";
        eventData.MessageId = Guid.NewGuid().ToString();
    }
}