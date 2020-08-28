using Alidu.Core.Domain.Interfaces;
using Alidu.CQRS.Handler;
using Alidu.CQRS;
using Alidu.CQRS.Interfaces;
using DomainServiceTemplate.Data;
using DomainServiceTemplate.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DomainServiceTemplate.Application.Commands
{
    public class CreateSampleCommandHandler : MessageHandler<CreateSampleCommand>, IRequestHandler<CreateSampleCommand>
    {
        private readonly ISampleRepository _repository;
        private readonly IMediator _mediator;

        public CreateSampleCommandHandler(IAggregateEventService aggregateEventService, IUnitOfWork unitOfWork, ISampleRepository repository, IMediator mediator) : base(aggregateEventService, unitOfWork)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreateSampleCommand request, CancellationToken cancellationToken)
        {
            await Handle(Guid.NewGuid().ToString(), request);
            return Unit.Value;
        }

        public override async Task MessageHandle(string correlationId, CreateSampleCommand command)
        {
            var sample = new Sample(command);
            await _repository.SnapshotSampleAsync(sample);
            await _mediator.DispatchPendingEventsAsync(new[] { sample });
        }
    }
}
