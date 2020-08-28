using Alidu.Core.Domain.Interfaces;
using Alidu.CQRS.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.CQRS.Handler
{
    public class AggregateEventHandler<TEvent> : INotificationHandler<TEvent> where TEvent : AggregateEvent
    {
        private readonly IAggregateEventService _aggregateEventService;
        private readonly IUnitOfWork _unitOfWork;

        public AggregateEventHandler(IAggregateEventService aggregateEventService, IUnitOfWork unitOfWork)
        {
            _aggregateEventService = aggregateEventService;
            _unitOfWork = unitOfWork;
        }

        public virtual async Task Handle(TEvent @event, CancellationToken cancellationToken)
        {
            @event.SetTransactionId(_unitOfWork.TransactionId);

            await _aggregateEventService.AddAndSaveEventAsync(@event, cancellationToken);
        }
    }
}