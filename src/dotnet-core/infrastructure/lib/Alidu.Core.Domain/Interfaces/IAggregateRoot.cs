using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.Core.Domain.Interfaces
{
    public interface IAggregateRoot
    {
        string Id { get; }
        int Version { get; }

        void SetId(string id);

        bool IsTransient();

        void ApplyEvent(IDomainEvent @event);

        IReadOnlyCollection<IDomainEvent> PendingEvents { get; }

        void ClearPendingEvents();
    }
}
