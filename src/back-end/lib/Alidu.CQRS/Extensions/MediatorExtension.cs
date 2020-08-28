using Alidu.Core.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.CQRS
{
    public static class MediatorExtension
    {
        public static async Task DispatchPendingEventsAsync<TAggregateRoot>(this IMediator mediator, IEnumerable<TAggregateRoot> aggregateRoots, CancellationToken cancellationToken = default) where TAggregateRoot : IAggregateRoot
        {
            var hasPendingEvents = aggregateRoots.Where(x => x.PendingEvents != null && x.PendingEvents.Any());

            var @events = hasPendingEvents
                .SelectMany(x => x.PendingEvents)
                .ToList();

            hasPendingEvents.ToList()
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