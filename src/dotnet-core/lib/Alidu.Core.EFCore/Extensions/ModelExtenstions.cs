using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq.Expressions;

namespace Alidu.Core.EFCore
{
    public static class ModelExtenstions
    {
        public static PropertyBuilder<TProperty> HasProperty<TEntity, TProperty>(this EntityTypeBuilder<TEntity> entity, Expression<Func<TEntity, TProperty>> propertyExpression) where TEntity : class
        {
            var memberName = ((MemberExpression)propertyExpression.Body).Member.Name;
            return entity.Property(propertyExpression).HasColumnName(memberName);
        }
    }
}