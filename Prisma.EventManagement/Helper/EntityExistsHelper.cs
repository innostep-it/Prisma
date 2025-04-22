using Azure.Messaging.ServiceBus.Administration;

namespace Prisma.EventManagement.Helper;

public static class EntityExistsHelper
{
    public static async Task CreateTopicIfNotExists(string conString, string queueName)
    {
        var adminClient = new ServiceBusAdministrationClient(conString);

        if (!await adminClient.TopicExistsAsync(queueName))
        {
            await adminClient.CreateTopicAsync(queueName);
        }
    }
}