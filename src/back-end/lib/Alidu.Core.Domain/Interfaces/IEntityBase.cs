using Alidu.Core.Domain.Interfaces;
using System;

namespace Alidu.Core.Domain.Interfaces
{
    public interface IEntityBase
    {
        string Id { get; }
        public string WorkingOrgId { get; }

        bool SoftDeleted { get; }
        void SetWorkingOrgId(string workingOrgId);
        void Delete();

    }
}