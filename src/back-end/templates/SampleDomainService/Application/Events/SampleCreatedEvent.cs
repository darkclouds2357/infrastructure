using Alidu.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleDomainService.Application.Events
{
    public class SampleCreatedEvent : AggregateEvent
    {
        public SampleCreatedEvent(string name, string description) : base(SampleEventEnum.NEW_SAMPLE_CREATED_EVENT)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
    }
}
