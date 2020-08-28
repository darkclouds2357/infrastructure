using Alidu.Core.Domain.Interfaces;
using Alidu.CQRS.Interfaces;
using Alidu.MongoDB;
using AutoMapper;
using SampleDomainService.Data.Schemas;
using SampleDomainService.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SampleDomainService.Data.Repository
{
    public class SampleRepository : ISampleRepository
    {
        private readonly ServiceContext _context;
        private readonly IMapper _mapper;
        private readonly IEventStoreProvider _eventStoreProvider;

        public SampleRepository(IUnitOfWork unitOfWork, IMapper mapper, IEventStoreProvider eventStoreProvider)
        {
            UnitOfWork = unitOfWork;
            _mapper = mapper;
            _eventStoreProvider = eventStoreProvider;
            _context = unitOfWork as ServiceContext;
        }

        public IUnitOfWork UnitOfWork { get; }

        public async Task<Sample> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var sampleSnapshot = await _context.Samples.Find(s => s.Id == id).FirstOrDefaultAsync(cancellationToken);
            if (sampleSnapshot == null)
                return null;
            var sample = _mapper.Map<Sample>(sampleSnapshot);

            var sampleEvents = await _eventStoreProvider.GetEventsAsync(sample.Id, sample.Version);

            foreach (var @event in sampleEvents)
            {
                sample.ApplyEvent(@event.GetPayload<IAggregateEvent>());
            }

            return sample;
        }

        public async Task SnapshotSampleAsync(Sample sample, CancellationToken cancellationToken = default)
        {
            var sampleSchema = _mapper.Map<SampleSchema>(sample);

            await _context.UpdateAsync(s => s.Id == sample.Id, sampleSchema, cancellationToken);
        }
    }
}
