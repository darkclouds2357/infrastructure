using Alidu.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleDomainService.Data.Schemas
{
    public class SampleSchema : IEntityBase, ITrackable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string WorkingOrgId { get; set; }

        public bool SoftDeleted { get; set; }
        public string CreatedByOrgId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedByOrgId { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string UpdatedBy { get; set; }


        public void Created(string orgId, DateTime createdDate, string createdBy)
        {
            CreatedByOrgId = orgId;
            CreatedDate = createdDate;
            CreatedBy = createdBy;
        }

        public void Delete() => SoftDeleted = true;

        public void SetWorkingOrgId(string workingOrgId) => WorkingOrgId = WorkingOrgId;

        public void Updated(string orgId, DateTime updatedDate, string updatedBy)
        {
            UpdatedBy = updatedBy;
            UpdatedByOrgId = UpdatedByOrgId;
            UpdatedDate = updatedDate;
        }
    }
}
