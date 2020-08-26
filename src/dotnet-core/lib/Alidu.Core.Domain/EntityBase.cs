using Alidu.Core.Domain.Interfaces;
using System;

namespace Alidu.Core.Domain
{
    public abstract class EntityBase : DomainBase
    {
        public string Id { get; private set; }
        public string WorkingOrgId { get; private set; }

        public bool SoftDeleted { get; protected set; }

        public void SetId(string id) => Id = id;

        public void SetWorkingOrgId(string workingOrgId) => WorkingOrgId = workingOrgId;

        public bool IsTransient()
        {
            return string.IsNullOrWhiteSpace(Id);
        }
    }

    public abstract class TrackableEntityBase : EntityBase, ITrackable
    {
        public string CreatedByOrgId { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public string CreatedBy { get; private set; }

        public string UpdatedByOrgId { get; private set; }
        public DateTime UpdatedDate { get; private set; }
        public string UpdatedBy { get; private set; }

        public void Created(string orgId, DateTime createdDate, string createdBy)
        {
            CreatedByOrgId = orgId;
            CreatedDate = createdDate;
            CreatedBy = createdBy;
        }

        public void Updated(string orgId, DateTime updatedDate, string updatedBy)
        {
            UpdatedByOrgId = orgId;
            UpdatedDate = updatedDate;
            UpdatedBy = updatedBy;
        }
    }

    public abstract class SimpleTrackableEntityBase : EntityBase, ISimpleTrackable
    {
        public string CreatedByOrgId { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public string CreatedBy { get; private set; }

        public void Created(string orgId, DateTime createdDate, string createdBy)
        {
            CreatedByOrgId = orgId;
            CreatedDate = createdDate;
            CreatedBy = createdBy;
        }
    }
}