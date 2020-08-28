using Alidu.MessageBus.Abstractions;
using Newtonsoft.Json;
using System;
using System.Data.SqlTypes;

namespace Alidu.CQRS
{
    public class EventStore
    {
        public EventStore()
        {
            TimesSent = 0;
            State = EventStateEnum.NotPublished;
        }

        public EventStore(AggregateEvent @event) : this()
        {
            AggregateId = @event.AggregateId;
            EventId = @event.Id;
            CreationTime = @event.CreationDate;
            Version = @event.Version;
            TransactionId = @event.TransactionId;
            Payload = @event;
            EventName = @event.MessageName;
        }

        public string AggregateId { get; set; }
        public Guid EventId { get; set; }
        public Guid TransactionId { get; set; }
        public string EventName { get; set; }
        public object Payload { get; set; }
        public EventStateEnum State { get; private set; }
        public int TimesSent { get; private set; }
        public DateTime CreationTime { get; set; }
        public int Version { get; set; }

        public T GetPayload<T>() where T : class
        {
            if (!(Payload is T result))
            {
                var payloadJson = JsonConvert.SerializeObject(Payload);
                result = JsonConvert.DeserializeObject<T>(payloadJson);
            }
            return result;

        }

        public void UpdateState(EventStateEnum status)
        {
            State = status;
            if (State == EventStateEnum.InProgress)
                TimesSent++;
        }
    }
}