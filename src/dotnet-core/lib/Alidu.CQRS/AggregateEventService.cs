using Alidu.CQRS.Interfaces;
using Alidu.MessageBus;
using Alidu.MessageBus.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.CQRS
{
    internal class AggregateEventService : IAggregateEventService
    {
        private readonly IMessageBus _messageBus;
        private readonly IEventStoreService _eventStoreService;

        public AggregateEventService(IMessageBus messageBus, IEventStoreService eventStoreService)
        {
            _messageBus = messageBus;
            _eventStoreService = eventStoreService;
        }

        public async Task AddAndSaveEventAsync(AggregateEvent integrationMessage, CancellationToken cancellationToken = default)
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
                    _messageBus.Publish(@event.EventName, @event.Payload);
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