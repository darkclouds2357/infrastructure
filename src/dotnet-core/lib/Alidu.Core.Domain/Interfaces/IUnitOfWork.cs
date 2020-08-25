using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.Core.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Guid TransactionId { get; }
        bool HasActiveTransaction { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task ExecutionTransactionStrategy(Func<Guid, CancellationToken, Task> onTransaction, Func<Guid, CancellationToken, Task> afterCommitTransction = null, CancellationToken cancellationToken = default);

        void Migrate();
    }
}
