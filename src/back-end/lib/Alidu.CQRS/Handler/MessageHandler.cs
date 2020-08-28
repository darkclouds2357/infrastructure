using Alidu.Core.Domain.Interfaces;
using Alidu.CQRS.Interfaces;
using Alidu.MessageBus.Abstractions;
using Alidu.MessageBus.Interfaces;
using System.Threading.Tasks;

namespace Alidu.CQRS.Handler
{
    public abstract class MessageHandler<TMessage> : IMessageHandler<TMessage>
        where TMessage : BaseMessage
    {
        private readonly IAggregateEventService _aggregateEventService;
        private readonly IUnitOfWork _unitOfWork;

        public MessageHandler(IAggregateEventService aggregateEventService, IUnitOfWork unitOfWork)
        {
            _aggregateEventService = aggregateEventService;
            _unitOfWork = unitOfWork;
        }

        public virtual async Task Handle(string correlationId, TMessage message)
        {
            await _unitOfWork.ExecutionTransactionStrategy(async (transactionId, cancellationToken) =>
            {
                // TODO: store to request message to Message Store

                await MessageHandle(correlationId, message);
            },
            afterCommitTransction: async (transactionId, cancellationToken) =>
            {
                await _aggregateEventService.PublishEventsThroughMessageBusAsync(transactionId);
            });
        }

        public abstract Task MessageHandle(string correlationId, TMessage message);
    }
}