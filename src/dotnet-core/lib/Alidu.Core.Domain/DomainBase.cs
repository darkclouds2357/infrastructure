using Alidu.Core.Domain.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Alidu.Core.Domain
{
    public abstract class DomainBase
    {
        public int Version { get; private set; } = 0;

        private LinkedList<IDomainEvent> _pendingEvents;

        [JsonIgnore]
        public IReadOnlyCollection<IDomainEvent> PendingEvents => _pendingEvents ?? new LinkedList<IDomainEvent>();

        [JsonIgnore]
        public LinkedList<IDomainEvent> AppliedEvents { get; private set; }

        public void AddEvent<TDomainEvent>(TDomainEvent eventItem, bool isApply = true) where TDomainEvent : IDomainEvent
        {
            _pendingEvents ??= new LinkedList<IDomainEvent>();
            _pendingEvents.AddLast(eventItem);
            if (!isApply)
                return;
            ApplyEvent(eventItem);
        }

        public void ApplyEvent<TDomainEvent>(TDomainEvent @event) where TDomainEvent : IDomainEvent
        {
            ((dynamic)this).Apply((dynamic)@event);
            Version++;

            @event.Version = Version;

            AppliedEvents ??= new LinkedList<IDomainEvent>();
            AppliedEvents.AddLast(@event);
        }

        public void RemovePendingEvent<TDomainEvent>(TDomainEvent eventItem) where TDomainEvent : IDomainEvent
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
