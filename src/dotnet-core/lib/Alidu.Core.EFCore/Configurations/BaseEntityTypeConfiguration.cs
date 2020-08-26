using Alidu.Core.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alidu.Core.EFCore.Configurations
{
    public static class BaseEntityTypeConfiguration
    {
        public static EntityTypeBuilder<TEntity> MapEntityBase<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : EntityBase
        {
            builder.HasKey(c => c.Id);
            builder.HasProperty(c => c.Id).ValueGeneratedOnAdd();
            builder.HasProperty(c => c.WorkingOrgId);

            builder.Ignore(c => c.PendingEvents);
            builder.Ignore(c => c.AppliedEvents);
            return builder;
        }

        public static EntityTypeBuilder<TEntity> MapEntitySimpleTrackable<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : SimpleTrackableEntityBase
        {
            builder.MapEntityBase();

            builder.HasProperty(c => c.CreatedDate);
            builder.HasProperty(c => c.CreatedBy);
            builder.HasProperty(c => c.CreatedByOrgId);

            return builder;
        }

        public static EntityTypeBuilder<TEntity> MapEntityTrackable<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : TrackableEntityBase
        {
            builder.MapEntityBase();

            builder.HasProperty(c => c.CreatedDate);
            builder.HasProperty(c => c.CreatedBy);
            builder.HasProperty(c => c.CreatedByOrgId);

            builder.HasProperty(c => c.UpdatedDate);
            builder.HasProperty(c => c.UpdatedBy);
            builder.HasProperty(c => c.UpdatedByOrgId);

            return builder;
        }
    }
}