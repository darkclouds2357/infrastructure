using Alidu.CQRS.Interfaces;
using Alidu.MessageBus;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.CQRS.ExecptionHandler
{
    public class ServiceExceptionEventHandler : INotificationHandler<ServiceExceptionEvent>
    {
        private readonly IMessageBus _messageBus;

        public ServiceExceptionEventHandler(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public virtual Task Handle(ServiceExceptionEvent @event, CancellationToken cancellationToken)
        {
            _messageBus.Publish(@event);
            return Task.CompletedTask;
        }
    }
}
