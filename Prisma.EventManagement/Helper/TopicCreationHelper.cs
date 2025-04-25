using Azure.Messaging.ServiceBus.Administration;

namespace Prisma.EventManagement.Helper;

public static class TopicCreationHelper
{
    public static async Task CreateTopicIfNotExists(string conString, string topicName, string subscriptionName)
    {
        var adminClient = new ServiceBusAdministrationClient(conString);

        if (!await adminClient.TopicExistsAsync(topicName))
        {
            await adminClient.CreateTopicAsync(topicName);
            await CreateSubscriptionIfNotExists(conString, topicName, subscriptionName);
        }
    }
    
    public static async Task CreateSubscriptionIfNotExists(string conString, string topicName, string subscriptionName)
    {
        var adminClient = new ServiceBusAdministrationClient(conString);

        if (!await adminClient.SubscriptionExistsAsync(topicName, subscriptionName))
        {
            await adminClient.CreateSubscriptionAsync(topicName, subscriptionName);
        }
    }
}