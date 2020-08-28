using Alidu.Core.Domain;
using Alidu.Core.Domain.Interfaces;
using Alidu.CQRS;
using SampleDomainService.Application.Commands;
using SampleDomainService.Application.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleDomainService.Models
{
    public class Sample : AggregateRoot, IEntityBase, ITrackable
    {
        public Sample()
        {
            SetId(Guid.NewGuid().ToString());
        }

        public Sample(CreateSampleCommand createSampleCommand) : this()
        {
            Name = createSampleCommand.Name;
            Description = createSampleCommand.Description;
            AddEvent(new SampleCreatedEvent(Name, Description));
        }

        public Sample(string id, int version, string name, string description, string createdByOrgId, DateTime createdDate, string createdBy, string updatedByOrgId, DateTime updatedDate, string updatedBy, string workingOrgId, bool softDeleted)
        {
            SetId(id);
            SetVersion(version);
            Name = name;
            Description = description;
            CreatedByOrgId = createdByOrgId;
            CreatedDate = createdDate;
            CreatedBy = createdBy;
            UpdatedByOrgId = updatedByOrgId;
            UpdatedDate = updatedDate;
            UpdatedBy = updatedBy;
            WorkingOrgId = workingOrgId;
            SoftDeleted = softDeleted;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }

        public string CreatedByOrgId { get; private set; }

        public DateTime CreatedDate { get; private set; }

        public string CreatedBy { get; private set; }

        public string UpdatedByOrgId { get; private set; }

        public DateTime UpdatedDate { get; private set; }

        public string UpdatedBy { get; private set; }

        public string WorkingOrgId { get; private set; }

        public bool SoftDeleted { get; private set; }

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

        public void Apply(SampleCreatedEvent @event)
        {
            Name = @event.Name;
            Description = @event.Description;
        }
    }
}
