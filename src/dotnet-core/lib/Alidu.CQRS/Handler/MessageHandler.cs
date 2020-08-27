using Alidu.Core.Domain.Interfaces;
using Alidu.CQRS.Interfaces;
using Alidu.MessageBus.Abstractions;
using Alidu.MessageBus.Interfaces;
using System.Threading.Tasks;

namespace Alidu.CQRS.Handler
{
    public abstract class MessageHandler<TEvent> : IMessageHandler<TEvent> where TEvent : BaseMessage
    {
        private readonly IAggregateEventService _aggregateEventService;
        private readonly IUnitOfWork _unitOfWork;

        public MessageHandler(IAggregateEventService aggregateEventService, IUnitOfWork unitOfWork)
        {
            _aggregateEventService = aggregateEventService;
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
                await _aggregateEventService.PublishEventsThroughMessageBusAsync(transactionId);
            });
        }

        public abstract Task MessageHandle(string correlationId, TEvent @event);
    }
}