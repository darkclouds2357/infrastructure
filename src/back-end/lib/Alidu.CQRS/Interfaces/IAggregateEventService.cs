using System;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.CQRS.Interfaces
{
    public interface IAggregateEventService
    {
        Task PublishEventsThroughMessageBusAsync(Guid transactionId, CancellationToken cancellationToken = default);

        Task AddAndSaveEventAsync(AggregateEvent integrationMessage, CancellationToken cancellationToken = default);
    }
}