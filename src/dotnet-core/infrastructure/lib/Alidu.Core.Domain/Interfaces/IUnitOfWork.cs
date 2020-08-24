using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Alidu.Core.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        bool HasActiveTransaction { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);

        void SetTransactionId(Guid transactionId);

        Task ExecutionTransactionStrategy(string typeName, Func<Guid, CancellationToken, Task> onTransaction, Func<Guid, CancellationToken, Task> afterCommitTransction = null, CancellationToken cancellationToken = default);

        void Migrate();
    }
}
