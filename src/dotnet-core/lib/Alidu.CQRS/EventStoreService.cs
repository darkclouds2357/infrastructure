using Alidu.Core.Domain;
using Alidu.CQRS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.CQRS
{
    class EventStoreService : IEventStoreService
    {
        private readonly IEventStoreProvider _eventStoreProvider;

        public EventStoreService(IEventStoreProvider eventStoreProvider)
        {
            _eventStoreProvider = eventStoreProvider;
        }
        public Task MarkEventAsFailedAsync(Guid eventId, CancellationToken cancellationToken = default) => UpdateEventStatus(eventId, EventStateEnum.PublishedFailed);

        public Task MarkEventAsInProgressAsync(Guid eventId, CancellationToken cancellationToken = default) => UpdateEventStatus(eventId, EventStateEnum.InProgress);

        public Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken = default) => UpdateEventStatus(eventId, EventStateEnum.Published);

        public async Task<IEnumerable<EventStore>> RetrieveEventsPendingToPublishAsync(Guid transactionId, CancellationToken cancellationToken = default)
        {
            var result = await _eventStoreProvider.GetEventsByTransactionIdAsync(transactionId, EventStateEnum.NotPublished, cancellationToken);

            return result.OrderBy(o => o.CreationTime);
        }

        public async Task SaveEventAsync(BaseMessage @event, CancellationToken cancellationToken = default)
        {
            var eventStoreEntry = new EventStore(@event);

            await _eventStoreProvider.AddAsync(eventStoreEntry, cancellationToken);
        }

        private async Task UpdateEventStatus(Guid eventId, EventStateEnum status)
        {
            var eventStoreEntry = await _eventStoreProvider.GetEventByIdAsync(eventId);

            eventStoreEntry.UpdateState(status);

            await _eventStoreProvider.UpdateAsync(eventStoreEntry);
        }
    }
}
