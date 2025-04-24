using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prisma.EventManagement.Configuration;
using Prisma.EventManagement.Helper;
using Prisma.EventManagement.Shared;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Prisma.EventManagement.Services.RabbitMq;

public class RabbitMqEventReceivingService<TEvent>(
    ILogger<RabbitMqEventReceivingService<TEvent>> logger,
    IOptions<EventManagementConfiguration> options,
    IServiceProvider serviceProvider)
    : BackgroundService where TEvent : BaseEvent
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("RabbitMQ Event Receiver is starting.");

        var factory = new ConnectionFactory()
        {
            Uri = new Uri(options.Value.RabbitMQConnectionString)
        };
        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        var topicInfo =
            TopicInformationHelper.GetTopicInfos<TEvent>(options.Value.ReceivingEventsConfiguration.ReceivingEvents);
        await channel.ExchangeDeclareAsync(exchange: topicInfo.Item1, type: ExchangeType.Fanout,
            cancellationToken: cancellationToken);

        // declare a server-named queue
        var queueDeclareResult = await channel.QueueDeclareAsync(cancellationToken: cancellationToken);
        var queueName = queueDeclareResult.QueueName;
        await channel.QueueBindAsync(queue: queueName, exchange: "logs", routingKey: string.Empty,
            cancellationToken: cancellationToken);


        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += ProcessMessageHandler;

        await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer,
            cancellationToken: cancellationToken);
    }

    private async Task ProcessMessageHandler(object obj, BasicDeliverEventArgs args)
    {
        logger.LogDebug("Message received. 'MassageId={messageId}'", args.BasicProperties.MessageId);

        try
        {
            var eventData = JsonSerializer.Deserialize<TEvent>(args.Body.Span, JsonConfig.DefaultOptions);

            if (eventData is null)
            {
                throw new JsonException("Deserialization failed");
            }

            logger.LogInformation("Receive message. 'MassageId={messageId}, MessageType={messageType}'",
                args.BasicProperties.MessageId,
                eventData.GetType().Name);

            using var scope = serviceProvider.CreateScope();
            var handler =
                scope.ServiceProvider.GetRequiredService(typeof(IEventHandler<TEvent>)) as IEventHandler<TEvent>;

            if (handler is null)
            {
                throw new InvalidOperationException($"No handler found for event type {typeof(TEvent).Name}");
            }

            await Task.Run(async () => await handler.ExecuteAsync(eventData));
        }
        catch (JsonException ex)
        {
            logger.LogError($"Error during deserialization: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex);
        }
    }
}