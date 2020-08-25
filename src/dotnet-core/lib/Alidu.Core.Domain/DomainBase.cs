using Newtonsoft.Json;
using System.Collections.Generic;

namespace Alidu.Core.Domain
{
    public abstract class DomainBase
    {
        public int Version { get; private set; } = 0;

        private LinkedList<IntegrationMessage> _pendingEvents;

        [JsonIgnore]
        public IReadOnlyCollection<IntegrationMessage> PendingEvents => _pendingEvents ?? new LinkedList<IntegrationMessage>();

        [JsonIgnore]
        public LinkedList<IntegrationMessage> AppliedEvents { get; private set; }

        public void AddEvent<TIntegrationEvent>(TIntegrationEvent eventItem, bool isApply = true) where TIntegrationEvent: IntegrationMessage
        {
            _pendingEvents ??= new LinkedList<IntegrationMessage>();
            _pendingEvents.AddLast(eventItem);
            if (!isApply)
                return;
            ApplyEvent(eventItem);
        }

        public void ApplyEvent<TIntegrationEvent>(TIntegrationEvent @event) where TIntegrationEvent : IntegrationMessage
        {
            ((dynamic)this).Apply((dynamic)@event);
            Version++;

            @event.Version = Version;

            AppliedEvents ??= new LinkedList<IntegrationMessage>();
            AppliedEvents.AddLast(@event);
        }

        public void RemovePendingEvent<TIntegrationEvent>(TIntegrationEvent eventItem) where TIntegrationEvent : IntegrationMessage
        {
            _pendingEvents?.Remove(eventItem);
        }

        public void ClearPendingEvents()
        {
            _pendingEvents?.Clear();
        }

        public void SetVersion(int version) => Version = version;
    }
}
