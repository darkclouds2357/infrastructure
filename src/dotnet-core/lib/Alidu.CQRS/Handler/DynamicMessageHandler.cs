using Alidu.Core.Domain.Interfaces;
using Alidu.CQRS.Interfaces;
using Alidu.MessageBus.Interfaces;
using System.Threading.Tasks;

namespace Alidu.CQRS.Handler
{
    public abstract class DynamicMessageHandler : IDynamicMessageHandler
    {
        private readonly IAggregateEventService _aggregateEventService;
        private readonly IUnitOfWork _unitOfWork;

        public DynamicMessageHandler(IAggregateEventService integrationMessageService, IUnitOfWork unitOfWork)
        {
            _aggregateEventService = integrationMessageService;
            _unitOfWork = unitOfWork;
        }

        public virtual async Task Handle(string correlationId, dynamic @event)
        {
            await _unitOfWork.ExecutionTransactionStrategy(async (transactionId, cancellationToken) =>
            {
                await DynamicMessageHandle(correlationId, @event);
            },
            afterCommitTransction: async (transactionId, cancellationToken) =>
            {
                await _aggregateEventService.PublishEventsThroughMessageBusAsync(transactionId);
            });
        }

        public abstract Task DynamicMessageHandle(string correlationId, dynamic @event);
    }
}