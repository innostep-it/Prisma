using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prisma.EventManagement.Configuration;
using Prisma.EventManagement.Helper;
using Prisma.EventManagement.Shared;

namespace Prisma.EventManagement.Services.EventReceivingService;

public class EventReceivingService<TEvent>(
    ILogger<EventReceivingService<TEvent>> logger,
    IOptions<EventManagementConfiguration> configOptions,
    IServiceProvider serviceProvider,
    ServiceBusProcessor serviceBusProcessor)
    : BackgroundService
    where TEvent : BaseEvent
{
    private readonly TimeSpan maxIdleTime = TimeSpan.FromMinutes(1);
    private DateTime lastMessageReceivedTime;
    private ServiceBusProcessor serviceBusProcessor = serviceBusProcessor;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("ServiceBusProcessor is starting");
        await InitializeServiceBusProcessorAsync(cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    private async Task InitializeServiceBusProcessorAsync(CancellationToken cancellationToken)
    {
        var options = new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 5,
            AutoCompleteMessages = false
        };

        if (QueueHelper.IsQueue<TEvent>(configOptions.Value.ReceivingEventsConfiguration.ReceivingEvents))
        {
            var queueName =
                QueueHelper.GetQueueName<TEvent>(configOptions.Value.ReceivingEventsConfiguration.ReceivingEvents);
            var conString = configOptions.Value.ServiceBusConnectionString.Replace("REPLACEENTITY", queueName);
            await QueueCreationHelper.CreateQueueIfNotExists(conString, queueName);
            var client = new ServiceBusClient(conString);
            serviceBusProcessor = client.CreateProcessor(queueName, options);
        }
        else
        {
            var topicInfos =
                TopicInformationHelper.GetTopicInfos<TEvent>(configOptions.Value.ReceivingEventsConfiguration
                    .ReceivingEvents);
            var conString = configOptions.Value.ServiceBusConnectionString.Replace("REPLACEENTITY", topicInfos.Item1);
            await TopicCreationHelper.CreateTopicIfNotExists(conString, topicInfos.Item1);
            var client = new ServiceBusClient(conString);
            serviceBusProcessor = client.CreateProcessor(topicInfos.Item1, topicInfos.Item2, options);
        }

        serviceBusProcessor.ProcessMessageAsync += ProcessMessageHandler;
        serviceBusProcessor.ProcessErrorAsync += ProcessErrorHandler;
        await serviceBusProcessor.StartProcessingAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogDebug("EventReceivingService Start ExecuteAsync");
        lastMessageReceivedTime = DateTime.UtcNow;

        while (!stoppingToken.IsCancellationRequested)
        {
            if (DateTime.UtcNow - lastMessageReceivedTime > maxIdleTime)
            {
                logger.LogInformation("Reinitializing ServiceBusProcessor due to inactivity. TEntity={TEntity}",
                    typeof(TEvent).Name);
                await serviceBusProcessor.StopProcessingAsync(stoppingToken);
                await InitializeServiceBusProcessorAsync(stoppingToken);
                lastMessageReceivedTime = DateTime.UtcNow;
            }

            await Task.Delay(1000, stoppingToken);
        }

        await serviceBusProcessor.StopProcessingAsync(stoppingToken);
    }

    private async Task ProcessMessageHandler(ProcessMessageEventArgs args)
    {
        logger.LogDebug("Message received. 'MassageId={messageId}'", args.Message.MessageId);
        lastMessageReceivedTime = DateTime.UtcNow;

        try
        {
            var eventData = JsonSerializer.Deserialize<TEvent>(args.Message.Body, JsonConfig.DefaultOptions);

            if (eventData is null)
            {
                throw new JsonException("Deserialization failed");
            }

            logger.LogInformation("Receive message. 'MassageId={messageId}, MessageType={messageType}'",
                args.Message.MessageId,
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
            await args.DeadLetterMessageAsync(args.Message, ex.Message);
        }
        finally
        {
            await args.CompleteMessageAsync(args.Message);
        }
    }

    private Task ProcessErrorHandler(ProcessErrorEventArgs args)
    {
        logger.LogError($"Error occurred: {args.Exception.Message}", args.Exception);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogWarning("ServiceBusProcessor is stopping. TEntity={TEntity}", typeof(TEvent).Name);
        await serviceBusProcessor.StopProcessingAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
        await serviceBusProcessor.DisposeAsync();
    }
}