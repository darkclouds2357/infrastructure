using Alidu.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.Core.MessageBus.Interfaces
{
    public interface IIntegrationMessageService
    {
        Task PublishEventsThroughMessageBusAsync(Guid transactionId, CancellationToken cancellationToken = default);

        Task AddAndSaveEventAsync(IntegrationMessage integrationMessage, CancellationToken cancellationToken = default);
    }
}
