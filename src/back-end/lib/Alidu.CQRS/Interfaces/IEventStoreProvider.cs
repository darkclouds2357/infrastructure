﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.CQRS.Interfaces
{
    public interface IEventStoreProvider
    {
        Task<EventStore> GetEventByIdAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task UpdateAsync(EventStore eventStoreEntry, CancellationToken cancellationToken = default);

        Task AddAsync(EventStore eventStoreEntry, CancellationToken cancellationToken = default);

        Task<IEnumerable<EventStore>> GetEventsByTransactionIdAsync(Guid transactionId, EventStateEnum eventState = EventStateEnum.NotPublished, CancellationToken cancellationToken = default);

        Task<IEnumerable<EventStore>> GetEventsAsync(string aggregateId, int fromVersion, CancellationToken cancellationToken = default);
    }
}