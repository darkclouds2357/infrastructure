using Alidu.Core.Domain.Interfaces;
using Alidu.MessageBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.CQRS
{
    public abstract class AggregateEvent : BaseMessage, IAggregateEvent
    {
        protected AggregateEvent(string eventName) : base(eventName)
        {
            EventName = eventName;
        }

        public string AggregateId { get; private set; }

        public int Version { get; private set; }

        public string EventName { get; }

        public void SetAggregateVersion(string aggregateId, int version)
        {
            AggregateId = aggregateId;
            Version = version;
        }
    }
}
