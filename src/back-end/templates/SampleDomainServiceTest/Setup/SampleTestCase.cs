using System.Collections.Generic;

namespace SampleDomainServiceTest.Setup
{
    public class SampleTestCase
    {
        public Sample Instance { get; set; }
        public IReadOnlyList<EventStore> InstanceEvents { get; set; }

        public object RequestCommand { get; set; }
        public IReadOnlyList<EventStore> PublishedEvents { get; set; }
    }
}