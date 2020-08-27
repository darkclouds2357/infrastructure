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
        public void AddEvent<TAggregateEvent>(TAggregateEvent eventItem, bool isApply = true) where TAggregateEvent : IAggregateEvent
        {
            _pendingEvents ??= new LinkedList<IAggregateEvent>();
            _pendingEvents.AddLast(eventItem);
            if (!isApply)
                return;
            ApplyEvent(eventItem);
        }

        public void ApplyEvent<TAggregateEvent>(TAggregateEvent @event) where TAggregateEvent : IAggregateEvent
        {
            ((dynamic)this).Apply((dynamic)@event);
            Version++;

            @event.SetAggregateVersion(Id, Version);

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