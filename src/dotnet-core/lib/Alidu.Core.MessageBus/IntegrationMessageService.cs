using Alidu.Core.Domain;
using Alidu.Core.IntegrationEvent.Interfaces;
using Alidu.Core.MessageBus.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.Core.MessageBus
{
    class IntegrationMessageService : IIntegrationMessageService
    {
        private readonly IMessageBus _messageBus;
        private readonly IEventStoreService _eventStoreService;

        public IntegrationMessageService(IMessageBus messageBus, IEventStoreService eventStoreService)
        {
            _messageBus = messageBus;
            _eventStoreService = eventStoreService;
        }
        public async Task AddAndSaveEventAsync(IntegrationMessage integrationMessage, CancellationToken cancellationToken = default)
        {
            await _eventStoreService.SaveEventAsync(integrationMessage, cancellationToken);
        }

        public async Task PublishEventsThroughMessageBusAsync(Guid transactionId, CancellationToken cancellationToken = default)
        {
            var pendingEvents = await _eventStoreService.RetrieveEventsPendingToPublishAsync(transactionId);

            foreach (var @event in pendingEvents)
            {
                try
                {
                    await _eventStoreService.MarkEventAsInProgressAsync(@event.EventId);
                    _messageBus.Publish(@event.Payload);
                    await _eventStoreService.MarkEventAsPublishedAsync(@event.EventId);
                }
                catch
                {
                    await _eventStoreService.MarkEventAsFailedAsync(@event.EventId);
                }
            }
        }
    }
}
