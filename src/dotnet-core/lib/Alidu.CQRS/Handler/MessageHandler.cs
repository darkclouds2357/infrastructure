using Alidu.Core.Domain.Interfaces;
using Alidu.CQRS.Interfaces;
using Alidu.MessageBus.Interfaces;
using System.Threading.Tasks;

namespace Alidu.CQRS.Handler
{
    public abstract class MessageHandler<TEvent> : IMessageHandler<TEvent> where TEvent : BaseMessage
    {
        private readonly IIntegrationMessageService _integrationMessageService;
        private readonly IUnitOfWork _unitOfWork;

        public MessageHandler(IIntegrationMessageService integrationMessageService, IUnitOfWork unitOfWork)
        {
            _integrationMessageService = integrationMessageService;
            _unitOfWork = unitOfWork;
        }

        public virtual async Task Handle(string correlationId, TEvent @event)
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