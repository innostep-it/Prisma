using Prisma.EventManagement.Services.RabbitMq;

namespace Prisma.EventManagement.Tests
{
    public class RabbitMqServiceManager
    {
        private readonly RabbitMqEventPublicationService publicationService;
        private readonly IServiceProvider serviceProvider;

        public RabbitMqServiceManager(
            RabbitMqEventPublicationService publicationService,
            IServiceProvider serviceProvider)
        {
            this.publicationService = publicationService;
            this.serviceProvider = serviceProvider;
        }

        public async Task RunAsync()
        {
            
            
            // Beispiel: Starte RabbitMQ-bezogene Dienste
            var receivingService = serviceProvider.GetService(typeof(RabbitMqEventReceivingService<>));
            // Logik für Start/Verwaltung hinzufügen
        }
    }
}