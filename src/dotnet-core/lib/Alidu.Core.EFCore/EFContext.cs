using Alidu.Common.Interfaces;
using Alidu.Core.Domain;
using Alidu.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.Core.EFCore
{
    public abstract class EFContext : DbContext, IUnitOfWork
    {
        private readonly IRequestHeader _requestHeader;
        private readonly ILogger _logger;
        private IDbContextTransaction _currentTransaction;

        public EFContext(DbContextOptions options) : base(options)
        {
        }

        public EFContext(DbContextOptions options, IRequestHeader requestHeader, ILogger logger) : this(options)
        {
            _requestHeader = requestHeader;
            _logger = logger;
        }

        public bool HasActiveTransaction => _currentTransaction != null;
        public Guid TransactionId => _currentTransaction?.TransactionId ?? Guid.NewGuid();

        public async Task ExecutionTransactionStrategy(Func<Guid, CancellationToken, Task> onTransaction, Func<Guid, CancellationToken, Task> afterCommitTransction = null, CancellationToken cancellationToken = default)
        {
            var strategy = Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                Guid transactionId;
                using (var transaction = await BeginTransactionAsync(cancellationToken))
                {
                    await onTransaction(transaction.TransactionId, cancellationToken);
                    await CommitTransactionAsync(transaction, cancellationToken);
                    transactionId = transaction.TransactionId;
                }
                if (afterCommitTransction != null)
                    await afterCommitTransction(transactionId, cancellationToken);
            });
        }

        private async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            return _currentTransaction;
        }

        private async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction)
                throw new InvalidOperationException($"DB Transaction {transaction.TransactionId} is not current transaction");

            try
            {
                await SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        private void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void Migrate() => Database.Migrate();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var ownerId = GetLoggedinOwnerId();
            var orgId = GetLoggedinOrgId();
            var workingOrgId = GetLoggedinWorkingOrgId();

            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                var now = DateTime.UtcNow;
                if (entry.Entity is ITrackable trackable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.Updated(orgId, now, ownerId);
                            break;

                        case EntityState.Added:
                            trackable.Created(orgId, now, ownerId);
                            break;
                    }
                }
                if (entry.Entity is ISimpleTrackable simpleTrackableEntity && entry.State == EntityState.Added)
                {
                    simpleTrackableEntity.Created(orgId, now, ownerId);
                }

                if (entry.Entity is EntityBase entityBase && string.IsNullOrWhiteSpace(entityBase.WorkingOrgId))
                {
                    entityBase.SetWorkingOrgId(workingOrgId);
                }
            }
        }

        private string GetLoggedinOrgId() => _requestHeader.Credential.OrgId;

        private string GetLoggedinOwnerId() => _requestHeader.Credential.OwnerId;

        private string GetLoggedinWorkingOrgId() => _requestHeader.Credential.WorkingOrgId;
    }
}