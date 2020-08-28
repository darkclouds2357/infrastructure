using Alidu.CQRS;
using Alidu.CQRS.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SampleDomainServiceTest.Setup
{
    public class FakeEventStoreProvider : IEventStoreProvider
    {
        public Task AddAsync(EventStore eventStoreEntry, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<EventStore> GetEventByIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EventStore>> GetEventsAsync(string aggregateId, int fromVersion, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<EventStore>> GetEventsByTransactionIdAsync(Guid transactionId, EventStateEnum eventState = EventStateEnum.NotPublished, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(EventStore eventStoreEntry, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}