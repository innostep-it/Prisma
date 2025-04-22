using Prisma.EventManagement.Shared;

namespace Prisma.EventManagement.Services.EventPublicationService;

public interface IEventPublicationService
{
    /// <summary>
    /// Publishes a single event of a specific type asynchronously. You not have to use DomainEvent as the type.
    /// Just use the EventType you want to publish.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event. Do not Use DomainEvent</typeparam>
    /// <param name="eventData">The event data to be published.</param>
    /// <param name="sessionId">Optional session ID for the event.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync<TEvent>(TEvent eventData, string? sessionId = null) where TEvent : BaseEvent;
    
    Task PublishBatchAsync<TEntity>(IEnumerable<TEntity> eventData, string? sessionId = null) where TEntity : BaseEvent;
}