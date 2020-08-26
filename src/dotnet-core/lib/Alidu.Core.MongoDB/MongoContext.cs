using Alidu.Common.Interfaces;
using Alidu.Core.Domain;
using Alidu.Core.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.Core.MongoDB
{
    public abstract class MongoContext : IUnitOfWork
    {
        internal enum State
        {
            Detached = 0,
            Unchanged = 1,
            Deleted = 2,
            Modified = 3,
            Added = 4
        }
        protected readonly IMongoDatabase Database;
        private readonly ILogger<MongoContext> _logger;
        private readonly IRequestCredential _requestCredential;
        private static Dictionary<Type, string> _entityName = new Dictionary<Type, string>();

        public MongoContext(IMongoDatabase mongoDatabase, ILogger<MongoContext> logger, IRequestCredential requestCredential)
        {
            Database = mongoDatabase;
            _logger = logger;
            _requestCredential = requestCredential;
        }

        public Guid TransactionId { get; private set; }

        public bool HasActiveTransaction { get; private set; }

        public void Dispose()
        {
        }

        public async Task ExecutionTransactionStrategy(Func<Guid, CancellationToken, Task> onTransaction, Func<Guid, CancellationToken, Task> afterCommitTransction = null, CancellationToken cancellationToken = default)
        {
            var policy = CreatePolicy();

            await policy.ExecuteAsync(async () =>
            {
                TransactionId = Guid.NewGuid();

                await onTransaction(TransactionId, cancellationToken);

                if (afterCommitTransction != null)
                    await afterCommitTransction(TransactionId, cancellationToken);
            });
        }
        protected void MapToCollection<T>(string name)
        {
            _entityName[typeof(T)] = name;
            if (!Database.ListCollectionNames().ToList().Any(c => c.Equals(name)))
                Database.CreateCollection(name);
            BsonClassMap.RegisterClassMap<T>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }

        protected void CreateCollection<T>(string name)
        {
            _entityName[typeof(T)] = name;
            if (!Database.ListCollectionNames().ToList().Any(c => c.Equals(name)))
                Database.CreateCollection(name);
        }

        protected void MapToCollection<T>(string name, Action<BsonClassMap<T>> classMapInitializer)
        {
            _entityName[typeof(T)] = name;
            if (!Database.ListCollectionNames().ToList().Any(c => c.Equals(name)))
                Database.CreateCollection(name);
            BsonClassMap.RegisterClassMap(classMapInitializer);
        }

        public abstract void Migrate();

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(1);
        }

        public IMongoCollection<TEntity> Set<TEntity>()
        {
            if (!_entityName.ContainsKey(typeof(TEntity)))
                throw new ArgumentNullException(nameof(TEntity));
            var entityName = _entityName[typeof(TEntity)];
            return Database.GetCollection<TEntity>(entityName);
        }

        public IMongoCollection<BsonDocument> Set(string entityName)
        {
            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentNullException(nameof(entityName));
            return Database.GetCollection<BsonDocument>(entityName);
        }

        private AsyncPolicy CreatePolicy(int retries = 3)
        {
            return Policy.Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        _logger.LogWarning(exception, "[Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
        internal void SetTrackable<TEntity>(TEntity entity, State state)
        {
            var userId = GetLoggedinOwnerId();
            var orgId = GetLoggedinOrgId();
            var workingOrgId = GetLoggedinWorkingOrgId();
            var now = DateTime.UtcNow;
            if (entity is ITrackable trackable)
            {
                switch (state)
                {
                    case State.Modified:
                        trackable.Updated(orgId, now, userId);
                        break;

                    case State.Added:
                        trackable.Created(orgId, now, userId);
                        break;
                }
            }
            if (entity is ISimpleTrackable simpleTrackableEntity)
            {
                simpleTrackableEntity.Created(orgId, now, userId);
            }

            if (entity is EntityBase entityBase && string.IsNullOrWhiteSpace(entityBase.WorkingOrgId))
            {
                entityBase.SetWorkingOrgId(workingOrgId);
            }
        }
        private string GetLoggedinOrgId() => _requestCredential.OrgId;

        private string GetLoggedinOwnerId() => _requestCredential.OwnerId;
        private string GetLoggedinWorkingOrgId() => _requestCredential.WorkingOrgId;
    }
}
