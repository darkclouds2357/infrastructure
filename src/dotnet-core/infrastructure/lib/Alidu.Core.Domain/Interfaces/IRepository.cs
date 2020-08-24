using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alidu.Core.Domain.Interfaces
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        IQueryable<T> Query();
    }
}
