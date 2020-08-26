using System;

namespace Alidu.Core.Domain.Interfaces
{
    public interface ITrackable
    {
        string CreatedByOrgId { get; }
        DateTime CreatedDate { get; }
        string CreatedBy { get; }
        string UpdatedByOrgId { get; }
        DateTime UpdatedDate { get; }
        string UpdatedBy { get; }

        void Created(string orgId, DateTime createdDate, string createdBy);

        void Updated(string orgId, DateTime updatedDate, string updatedBy);
    }
}