namespace Prisma.EventManagement.Shared;

public abstract class BaseEvent
{
    public string MessageId { get; set; }
    public string CorrelationId { get; set; }
    // Zeit in der Warteschlange
    public DateTimeOffset EnqueuedTime { get; set; }
}