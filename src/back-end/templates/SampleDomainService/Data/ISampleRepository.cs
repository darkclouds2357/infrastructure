using Alidu.Core.Domain.Interfaces;
using SampleDomainService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SampleDomainService.Data
{
    public interface ISampleRepository : IRepository<Sample>
    {
        Task SnapshotSampleAsync(Sample sample, CancellationToken cancellationToken = default);
    }
}
