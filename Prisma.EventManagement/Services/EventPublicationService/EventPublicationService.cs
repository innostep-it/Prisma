using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prisma.EventManagement.Configuration;
using Prisma.EventManagement.Helper;
using Prisma.EventManagement.Shared;

namespace Prisma.EventManagement.Services.EventPublicationService;

public class ServiceBusEventPublicationService(
    ILogger<ServiceBusEventPublicationService> logger,
    IOptions<EventManagementConfiguration> options)
    : IEventPublicationService
{
    public async Task PublishAsync<TEntity>(TEntity eventData, string? sessionId = null) where TEntity : BaseEvent
    {
        logger.LogDebug("Will publish event of type {EventType} to queue", nameof(TEntity));

        BasePropertiesHelper.SetBaseEventProperties(eventData, DateTimeOffset.UtcNow);

        var entityName = EntityNameHelper.GetEntityName(typeof(TEntity).Name, options.Value.SendingEventsConfiguration.SendingEvents);
        
        var queueConnectionString = options.Value.ServiceBusConnectionString.Replace("REPLACEENTITY", entityName);

        await EntityExistsHelper.CreateTopicIfNotExists(queueConnectionString, entityName);

        await SendMessage(queueConnectionString, entityName, eventData, sessionId);
    }
    
    public async Task PublishBatchAsync<TEntity>(IEnumerable<TEntity> eventData, string? sessionId = null) where TEntity : BaseEvent
    {
        logger.LogDebug("Will publish event of type {EventType} to queue", nameof(TEntity));

        foreach (var eventGroup in eventData.GroupBy(ed => ed.GetType()))
        {
            var entityName = EntityNameHelper.GetEntityName(typeof(TEntity).Name, options.Value.SendingEventsConfiguration.SendingEvents);

            var queueConnectionString = options.Value.ServiceBusConnectionString.Replace("REPLACEENTITY", entityName);

            var client = new ServiceBusClient(queueConnectionString);
            var sender = client.CreateSender(entityName);

            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var messages = eventGroup.Select(data =>
            {
                BasePropertiesHelper.SetBaseEventProperties(data, DateTimeOffset.UtcNow);
                var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, serializeOptions));
                return sessionId != null ? new ServiceBusMessage(messageBody) { SessionId = sessionId } : new ServiceBusMessage(messageBody);
            }).ToList();

            try
            {
                await sender.SendMessagesAsync(messages);
            }
            finally
            {
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }

    private async Task SendMessage<TEntity>(string queueConnectionString, string entityName, TEntity eventData, string? sessionId = null)
    {
        var client = new ServiceBusClient(queueConnectionString);
        var sender = client.CreateSender(entityName);

        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        ServiceBusMessage message;

        var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventData, serializeOptions));

        if (sessionId != null)
            message = new ServiceBusMessage(messageBody)
            {
                SessionId = sessionId
            };
        else
            message = new ServiceBusMessage(messageBody);

        try
        {
            await sender.SendMessageAsync(message);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
        finally
        {
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
    }
}