using Alidu.Core.Domain;
using Alidu.Core.Domain.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.MongoDB
{
    public static class MongoContextExtension
    {
        public static TEntity Add<TEntity>(this MongoContext mongoContext, TEntity entity)
        {
            mongoContext.SetTrackable(entity, MongoContext.State.Added);
            var entityCollection = mongoContext.Set<TEntity>();
            entityCollection.InsertOne(entity);

            return entity;
        }

        public static async Task<TEntity> AddAsync<TEntity>(this MongoContext mongoContext, TEntity entity, CancellationToken cancellationToken = default)
        {
            mongoContext.SetTrackable(entity, MongoContext.State.Added);
            var entityCollection = mongoContext.Set<TEntity>();
            await entityCollection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            return entity;
        }

        public static IEnumerable<TEntity> AddRange<TEntity>(this MongoContext mongoContext, IEnumerable<TEntity> entities)
        {
            var result = new List<TEntity>();
            foreach (var entity in entities)
            {
                mongoContext.SetTrackable(entity, MongoContext.State.Added);
                result.Add(entity);
            }
            var entityCollection = mongoContext.Set<TEntity>();

            entityCollection.InsertMany(result);
            return result;
        }

        public static async Task<IEnumerable<TEntity>> AddRangeAsync<TEntity>(this MongoContext mongoContext, IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            var result = new List<TEntity>();
            foreach (var entity in entities)
            {
                mongoContext.SetTrackable(entity, MongoContext.State.Added);
                result.Add(entity);
            }
            var entityCollection = mongoContext.Set<TEntity>();

            await entityCollection.InsertManyAsync(result, cancellationToken: cancellationToken);

            return result;
        }

        public static TEntity Update<TEntity>(this MongoContext mongoContext, Expression<Func<TEntity, bool>> filterExpression, TEntity entity)
        {
            var entityCollection = mongoContext.Set<TEntity>();
            var filter = Builders<TEntity>.Filter.Where(filterExpression);

            if (entity is ISimpleTrackable simpleTrackableEntityBase)
            {
                var existedEnity = entityCollection.Find(filter).FirstOrDefault();
                if (existedEnity != null && existedEnity is ISimpleTrackable existedSimpleTrackableEntity)
                    simpleTrackableEntityBase.Created(existedSimpleTrackableEntity.CreatedByOrgId, existedSimpleTrackableEntity.CreatedDate, existedSimpleTrackableEntity.CreatedBy);
            }
            else if (entity is ITrackable trackableEntityBase)
            {
                var existedEnity = entityCollection.Find(filter).FirstOrDefault();
                if (existedEnity != null && existedEnity is ITrackable existedTrackableEntity)
                    trackableEntityBase.Created(existedTrackableEntity.CreatedByOrgId, existedTrackableEntity.CreatedDate, existedTrackableEntity.CreatedBy);
            }

            mongoContext.SetTrackable(entity, MongoContext.State.Modified);

            entityCollection.ReplaceOne(filter, entity, options: new ReplaceOptions { IsUpsert = true });

            entity = entityCollection.Find(filterExpression).FirstOrDefault();
            return entity;
        }

        public static TEntity Update<TEntity>(this MongoContext mongoContext, Expression<Func<TEntity, bool>> filterExpression, UpdateDefinition<TEntity> updateDefinition)
        {
            var entityCollection = mongoContext.Set<TEntity>();
            updateDefinition = mongoContext.CreateUpdateInstance(updateDefinition);

            entityCollection.FindOneAndUpdate(filterExpression, updateDefinition);

            var entity = entityCollection.Find(filterExpression).FirstOrDefault();
            return entity;
        }

        public static async Task<TEntity> UpdateAsync<TEntity>(this MongoContext mongoContext, Expression<Func<TEntity, bool>> filterExpression, UpdateDefinition<TEntity> updateDefinition, CancellationToken cancellationToken = default)
        {
            var entityCollection = mongoContext.Set<TEntity>();
            updateDefinition = mongoContext.CreateUpdateInstance(updateDefinition);

            await entityCollection.FindOneAndUpdateAsync(filterExpression, updateDefinition, cancellationToken: cancellationToken);
            var entity = await entityCollection.Find(filterExpression).FirstOrDefaultAsync();

            return entity;
        }

        public static async Task<TEntity> UpdateAsync<TEntity>(this MongoContext mongoContext, Expression<Func<TEntity, bool>> filterExpression, TEntity entity, CancellationToken cancellationToken = default)
        {
            var entityCollection = mongoContext.Set<TEntity>();
            var filter = Builders<TEntity>.Filter.Where(filterExpression);

            if (entity is ISimpleTrackable simpleTrackableEntityBase)
            {
                var existedEnity = await entityCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
                if (existedEnity != null && existedEnity is ISimpleTrackable existedSimpleTrackableEntity)
                    simpleTrackableEntityBase.Created(existedSimpleTrackableEntity.CreatedByOrgId, existedSimpleTrackableEntity.CreatedDate, existedSimpleTrackableEntity.CreatedBy);
            }
            else if (entity is ITrackable trackableEntityBase)
            {
                var existedEnity = await entityCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
                if (existedEnity != null && existedEnity is ITrackable existedTrackableEntity)
                    trackableEntityBase.Created(existedTrackableEntity.CreatedByOrgId, existedTrackableEntity.CreatedDate, existedTrackableEntity.CreatedBy);
            }

            mongoContext.SetTrackable(entity, MongoContext.State.Modified);

            await entityCollection.ReplaceOneAsync(filter, entity, options: new ReplaceOptions { IsUpsert = true }, cancellationToken: cancellationToken);
            entity = await entityCollection.Find(filter).FirstOrDefaultAsync();

            return entity;
        }

        private static UpdateDefinition<TEntity> CreateUpdateInstance<TEntity>(this MongoContext mongoContext, UpdateDefinition<TEntity> updateDefinition)
        {
            var entity = Activator.CreateInstance<TEntity>();
            mongoContext.SetTrackable(entity, MongoContext.State.Modified);

            if (entity is ITrackable trackable)
            {
                updateDefinition = updateDefinition
                    .Set(nameof(trackable.UpdatedDate), trackable.UpdatedDate);
            }

            return updateDefinition;
        }
    }
}