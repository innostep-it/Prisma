using Azure.Messaging.ServiceBus.Administration;

namespace Prisma.EventManagement.Helper;

public static class QueueCreationHelper
{
    public static async Task CreateQueueIfNotExists(string conString, string queueName)
    {
        var adminClient = new ServiceBusAdministrationClient(conString);

        if (!await adminClient.QueueExistsAsync(queueName))
        {
            await adminClient.CreateQueueAsync(queueName);
        }
    }
}