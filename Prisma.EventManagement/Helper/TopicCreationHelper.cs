using Azure.Messaging.ServiceBus.Administration;

namespace Prisma.EventManagement.Helper;

public static class TopicCreationHelper
{
    public static async Task CreateTopicIfNotExists(string conString, string topicName)
    {
        var adminClient = new ServiceBusAdministrationClient(conString);

        if (!await adminClient.TopicExistsAsync(topicName))
        {
            await adminClient.CreateTopicAsync(topicName);
        }
    }
}