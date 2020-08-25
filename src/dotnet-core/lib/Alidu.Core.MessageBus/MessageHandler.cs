using Alidu.Core.Domain;
using Alidu.Core.Domain.Interfaces;
using Alidu.Core.MessageBus.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Alidu.Core.MessageBus
{
    public abstract class MessageHandler<TEvent> : IMessageHandler<TEvent> where TEvent : IntegrationMessage
    {
        private readonly IIntegrationMessageService _integrationMessageService;
        private readonly IUnitOfWork _unitOfWork;

        public MessageHandler(IIntegrationMessageService integrationMessageService, IUnitOfWork unitOfWork)
        {
            _integrationMessageService = integrationMessageService;
            _unitOfWork = unitOfWork;
        }
        public async Task Handle(string correlationId, TEvent @event)
        {
            await _unitOfWork.ExecutionTransactionStrategy(async (transactionId, cancellationToken) =>
            {
                await MessageHandle(correlationId, @event);
            },
            afterCommitTransction: async (transactionId, cancellationToken) =>
            {
                await _integrationMessageService.PublishEventsThroughMessageBusAsync(transactionId);
            });
        }

        public abstract Task MessageHandle(string correlationId, TEvent @event);
    }
}
