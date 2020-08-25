using Alidu.Core.Domain;
using Alidu.Core.Domain.Interfaces;
using Alidu.Core.MessageBus.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.Core.MessageBus
{
    public class IntegrationEventHandler<TEvent> : INotificationHandler<TEvent> where TEvent : IntegrationMessage
    {
        private readonly IIntegrationMessageService _integrationMessageService;
        private readonly IUnitOfWork _unitOfWork;

        public IntegrationEventHandler(IIntegrationMessageService integrationMessageService, IUnitOfWork unitOfWork)
        {
            _integrationMessageService = integrationMessageService;
            _unitOfWork = unitOfWork;
        }

        public virtual async Task Handle(TEvent @event, CancellationToken cancellationToken)
        {
            @event.SetTransactionId(_unitOfWork.TransactionId);

            await _integrationMessageService.AddAndSaveEventAsync(@event, cancellationToken);
        }
    }
}
