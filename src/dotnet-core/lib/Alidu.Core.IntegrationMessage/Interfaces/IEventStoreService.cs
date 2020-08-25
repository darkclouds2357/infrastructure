using Alidu.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.Core.IntegrationEvent.Interfaces
{
    public interface IEventStoreService
    {
        Task<IEnumerable<EventStore>> RetrieveEventsPendingToPublishAsync(Guid transactionId, CancellationToken cancellationToken = default);
        Task SaveEventAsync(IntegrationMessage @event, CancellationToken cancellationToken = default);
        Task MarkEventAsPublishedAsync(Guid eventId, CancellationToken cancellationToken = default);
        Task MarkEventAsInProgressAsync(Guid eventId, CancellationToken cancellationToken = default);
        Task MarkEventAsFailedAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}
