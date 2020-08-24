using Alidu.Core.Domain.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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

        public void AddEvent(IDomainEvent eventItem, bool isApply = true)
        {
            _pendingEvents ??= new LinkedList<IDomainEvent>();
            _pendingEvents.AddLast(eventItem);
            if (!isApply)
                return;
            ApplyEvent(eventItem);
        }

        public void ApplyEvent(IDomainEvent @event)
        {
            ((dynamic)this).Apply((dynamic)@event);
            Version++;

            @event.Version = Version;

            AppliedEvents ??= new LinkedList<IDomainEvent>();
            AppliedEvents.AddLast(@event);
        }

        public void RemovePendingEvent(IDomainEvent eventItem)
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
