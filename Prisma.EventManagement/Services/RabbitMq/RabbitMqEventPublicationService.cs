using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prisma.EventManagement.Configuration;
using Prisma.EventManagement.Helper;
using Prisma.EventManagement.Shared;
using RabbitMQ.Client;

namespace Prisma.EventManagement.Services.RabbitMq;

public class RabbitMqEventPublicationService(
    ILogger<RabbitMqEventPublicationService> logger,
    IOptions<EventManagementConfiguration> options)
    : IEventPublicationService
{
    public async Task PublishAsync<TEntity>(TEntity eventData, string? routingKey = null) where TEntity : BaseEvent
    {
        logger.LogDebug("Will publish event of type {EventType} to RabbitMQ", nameof(TEntity));

        BasePropertiesHelper.SetBaseEventProperties(eventData, DateTimeOffset.UtcNow);

        var entityName = EntityNameHelper.GetEntityName(typeof(TEntity).Name, options.Value.SendingEventsConfiguration.SendingEvents);

        var factory = new ConnectionFactory
        {
            Uri = new Uri(options.Value.RabbitMQConnectionString)
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(entityName, type: ExchangeType.Fanout);

        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventData, serializeOptions));

        await channel.BasicPublishAsync(
            exchange: entityName,
            routingKey: routingKey ?? string.Empty,
            body: messageBody);
    }

    public Task PublishBatchAsync<TEntity>(IEnumerable<TEntity> eventData, string? routingKey = null) where TEntity : BaseEvent
    {
        throw new NotImplementedException("Batch publishing is not implemented for RabbitMQ.");
    }
}