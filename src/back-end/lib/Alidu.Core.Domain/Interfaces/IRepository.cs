using System.Threading;
using System.Threading.Tasks;

namespace Alidu.Core.Domain.Interfaces
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        //IQueryable<T> Query();

        Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}