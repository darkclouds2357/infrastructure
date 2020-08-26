using Alidu.Core.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.CQRS
{
    public static class MediatorExtension
    {
        public static async Task DispatchPendingEventsAsync<TAggregateRoot>(this IMediator mediator, IEnumerable<TAggregateRoot> domains, CancellationToken cancellationToken = default) where TAggregateRoot : IAggregateRoot
        {
            var domainEntities = domains.Where(x => x.PendingEvents != null && x.PendingEvents.Any());

            var @events = domainEntities
                .SelectMany(x => x.PendingEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.ClearPendingEvents());

            var tasks = @events
                .Select(async (@event) =>
                {
                    await mediator.Publish(@event, cancellationToken);
                });

            await Task.WhenAll(tasks);
        }
    }
}
