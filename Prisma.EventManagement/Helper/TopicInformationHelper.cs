using Prisma.EventManagement.Configuration;

namespace Prisma.EventManagement.Helper;

public static class TopicInformationHelper
{
    public static Tuple<string, string> GetTopicInfos<TEvent>(List<ReceivingEventConfiguration> receivingEvents)
    {
        var name = receivingEvents.First(e => e.EventName == typeof(TEvent).Name).TopicName!;
        var subscription = receivingEvents.First(e => e.EventName == typeof(TEvent).Name).TopicSubscription!;

        return new Tuple<string, string>(name, subscription);
    }
}