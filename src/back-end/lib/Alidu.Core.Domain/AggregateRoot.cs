using Alidu.Core.Domain.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Alidu.Core.Domain
{
    public abstract class AggregateRoot : IAggregateRoot
    {
        public string Id { get; private set; }
        public int Version { get; private set; } = 0;

        private LinkedList<IAggregateEvent> _pendingEvents;

        [JsonIgnore]
        public IReadOnlyCollection<IAggregateEvent> PendingEvents => _pendingEvents ?? new LinkedList<IAggregateEvent>();

        [JsonIgnore]
        public LinkedList<IAggregateEvent> AppliedEvents { get; private set; }

        public void SetId(string id) => Id = id;

        public void AddEvent<TAggregateEvent>(TAggregateEvent @event, bool isFromHistory = false) where TAggregateEvent : IAggregateEvent
        {
            ApplyEvent(@event);
            @event.SetAggregateVersion(Id, Version);
            if (isFromHistory)
                return;
            _pendingEvents ??= new LinkedList<IAggregateEvent>();
            _pendingEvents.AddLast(@event);
        }

        public void ApplyEvent<TAggregateEvent>(TAggregateEvent @event) where TAggregateEvent : IAggregateEvent
        {
            ((dynamic)this).Apply((dynamic)@event);
            Version++;

            AppliedEvents ??= new LinkedList<IAggregateEvent>();
            AppliedEvents.AddLast(@event);
        }

        public void RemovePendingEvent<TAggregateEvent>(TAggregateEvent eventItem) where TAggregateEvent : IAggregateEvent
        {
            _pendingEvents?.Remove(eventItem);
        }

        public void ClearPendingEvents()
        {
            _pendingEvents?.Clear();
        }

        public void SetVersion(int version) => Version = version;

        public bool IsTransient() => string.IsNullOrWhiteSpace(Id);
    }
}