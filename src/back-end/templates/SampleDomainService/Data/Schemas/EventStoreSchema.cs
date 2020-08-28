using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleDomainService.Data.Schemas
{
    public class EventStoreSchema
    {
        public string AggregateId { get; set; }
        public Guid EventId { get; set; }
        public Guid TransactionId { get; set; }
        public string EventName { get; set; }
        public object Payload { get; set; }
        public string State { get; set; }
        public int TimesSent { get; set; }
        public DateTime CreationTime { get; set; }
        public int Version { get; set; }
    }
}
