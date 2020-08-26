using System.Linq;

namespace Alidu.Core.Domain.Interfaces
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        IQueryable<T> Query();
    }
}