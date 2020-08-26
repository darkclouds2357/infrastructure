using Alidu.Core.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Alidu.Core.Domain
{
    public static class AggregateRootExtension
    {
        public static IEnumerable<object> GetPendingEvents(this IAggregateRoot aggregate)
        {
            if (aggregate is DomainBase domain)
                return domain.PendingEvents;

            return Enumerable.Empty<object>();
        }

        public static string GetIdentifier(this IAggregateRoot aggregate) => $"{aggregate.GetType().Name}.{aggregate.Id}";
    }
}