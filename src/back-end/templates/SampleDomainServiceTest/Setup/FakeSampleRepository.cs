using Alidu.Core.Domain.Interfaces;
using SampleDomainService.Data;
using SampleDomainService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleDomainServiceTest.Setup
{
    public class FakeSampleRepository : ISampleRepository
    {
        public FakeSampleRepository()
        {

        }
        public IUnitOfWork UnitOfWork => throw new NotImplementedException();

        public Task<Sample> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SnapshotSampleAsync(Sample sample, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
