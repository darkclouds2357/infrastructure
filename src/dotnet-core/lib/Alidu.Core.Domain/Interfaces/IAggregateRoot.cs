using System.Collections.Generic;

namespace Alidu.Core.Domain.Interfaces
{
    public interface IAggregateRoot
    {
        string Id { get; }
        int Version { get; }

        void SetId(string id);

        bool IsTransient();
        void ApplyEvent<TIntegrationEvent>(TIntegrationEvent @event) where TIntegrationEvent : IntegrationMessage;

        IReadOnlyCollection<IntegrationMessage> PendingEvents { get; }

        void ClearPendingEvents();
    }
}
