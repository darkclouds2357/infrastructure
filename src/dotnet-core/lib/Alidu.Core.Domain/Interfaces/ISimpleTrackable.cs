using System;

namespace Alidu.Core.Domain.Interfaces
{
    public interface ISimpleTrackable
    {
        string CreatedByOrgId { get; }
        DateTime CreatedDate { get; }

        string CreatedBy { get; }

        bool SoftDeleted { get; }

        void Created(string orgId, DateTime createdDate, string createdBy);
    }
}