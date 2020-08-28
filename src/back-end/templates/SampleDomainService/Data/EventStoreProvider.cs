using Alidu.Core.Domain.Interfaces;
using Alidu.CQRS;
using Alidu.CQRS.Interfaces;
using Alidu.MongoDB;
using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using SampleDomainService.Data.Schemas;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SampleDomainService.Data
{
    public class EventStoreProvider : IEventStoreProvider
    {
        private readonly ServiceContext _context;
        private readonly IMapper _mapper;

        public EventStoreProvider(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _context = unitOfWork as ServiceContext;
            _mapper = mapper;
        }

        public async Task AddAsync(EventStore eventStoreEntry, CancellationToken cancellationToken = default)
        {
            var eventStoreSchema = _mapper.Map<EventStoreSchema>(eventStoreEntry);
            await _context.AddAsync(eventStoreSchema, cancellationToken);
        }

        public async Task<EventStore> GetEventByIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var eventStoreSchema = await _context.EventStores.Find(e => e.EventId == eventId).FirstOrDefaultAsync(cancellationToken);
            if (eventStoreSchema == null)
                return null;
            return _mapper.Map<EventStore>(eventStoreSchema);
        }

        public async Task<IEnumerable<EventStore>> GetEventsAsync(string aggregateId, int fromVersion, CancellationToken cancellationToken = default)
        {
            var aggregateFilter = new BsonDocument
                {
                    {
                        $"{nameof(EventStore.AggregateId)}", aggregateId
                    },
                    {
                        $"{nameof(EventStore.Version)}", new BsonDocument("$gt", BsonValue.Create(fromVersion))
                    }
                };

            var events = await _context.EventStores.Find(aggregateFilter).ToListAsync(cancellationToken);

            return _mapper.Map<List<EventStore>>(events);
        }

        public async Task<IEnumerable<EventStore>> GetEventsByTransactionIdAsync(Guid transactionId, EventStateEnum eventState = EventStateEnum.NotPublished, CancellationToken cancellationToken = default)
        {
            var aggregateFilter = new BsonDocument
                {
                    {
                        $"{nameof(EventStore.TransactionId)}", BsonValue.Create(transactionId)
                    },
                    {
                        $"{nameof(EventStore.State)}", BsonValue.Create(eventState.ToString())
                    }
                };

            var events = await _context.EventStores.Find(aggregateFilter).ToListAsync(cancellationToken);
            return _mapper.Map<List<EventStore>>(events);
        }

        public async Task UpdateAsync(EventStore eventStoreEntry, CancellationToken cancellationToken = default)
        {
            var eventStoreSchema = _mapper.Map<EventStoreSchema>(eventStoreEntry);

            await _context.UpdateAsync(e => e.AggregateId == eventStoreEntry.AggregateId && e.EventId == e.EventId, eventStoreSchema, cancellationToken);
        }
    }
}