namespace Prisma.EventManagement.Shared;

public interface IEventHandler<in TEvent>
{
    Task ExecuteAsync(TEvent message);
}