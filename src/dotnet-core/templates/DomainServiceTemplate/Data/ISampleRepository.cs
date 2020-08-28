using Alidu.Core.Domain.Interfaces;
using DomainServiceTemplate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DomainServiceTemplate.Data
{
    public interface ISampleRepository : IRepository<Sample>
    {
        Task SnapshotSampleAsync(Sample sample, CancellationToken cancellationToken = default);
    }
}
