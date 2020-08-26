using System;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.CQRS.Interfaces
{
    public interface IIntegrationMessageService
    {
        Task PublishEventsThroughMessageBusAsync(Guid transactionId, CancellationToken cancellationToken = default);

        Task AddAndSaveEventAsync(BaseMessage integrationMessage, CancellationToken cancellationToken = default);
    }
}