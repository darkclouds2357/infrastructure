using Alidu.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Alidu.CQRS
{
    public class EventStore
    {
        private EventStore() { }
        public EventStore(BaseMessage @event)
        {
            EventId = @event.Id;
            CreationTime = @event.CreationDate;
            Version = @event.Version;
            State = EventStateEnum.NotPublished;
            TimesSent = 0;
            TransactionId = @event.TransactionId;
        }
        public Guid EventId { get; private set; }
        public Guid TransactionId { get; private set; }
        public BaseMessage Payload { get; private set; }
        public EventStateEnum State { get; private set; }
        public int TimesSent { get; private set; }
        public DateTime CreationTime { get; private set; }
        public int Version { get; private set; }

        public void UpdateState(EventStateEnum status)
        {
            State = status;
            if (State == EventStateEnum.InProgress)
                TimesSent++;
        }
    }
}
