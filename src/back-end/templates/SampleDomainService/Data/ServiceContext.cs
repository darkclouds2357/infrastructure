using Alidu.Common.Interfaces;
using Alidu.MongoDB;
using SampleDomainService.Application.Events;
using SampleDomainService.Data.Schemas;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleDomainService.Data
{
    public class ServiceContext : MongoContext
    {
        public ServiceContext(IMongoDatabase mongoDatabase, ILogger<MongoContext> logger, IRequestHeader requestHeader) : base(mongoDatabase, logger, requestHeader)
        {
        }

        public IMongoCollection<EventStoreSchema> EventStores => Set<EventStoreSchema>();

        public IMongoCollection<SampleSchema> Samples => Set<SampleSchema>();

        public override void Migrate()
        {
            MapToCollection<EventStoreSchema>(nameof(EventStoreSchema));
            MapToCollection<SampleSchema>(nameof(SampleSchema));

            RegisterClassMap<SampleCreatedEvent>();
        }
    }
}
