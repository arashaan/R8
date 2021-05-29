using Humanizer;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using R8.EntityFrameworkCore.Audits;

namespace R8.EntityFrameworkCore
{
    internal static class EntityBaseExtensions
    {
        internal static EntityTypeBuilder<TEntity> ApplyIdentifierConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IEntityBaseIdentifier
        {
            builder.ToTable(Extensions.GetTableName(typeof(TEntity)));
            builder.HasKey(x => x.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            return builder;
        }

        internal static EntityTypeBuilder<TEntity> ApplyAuditConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IEntityBaseAudit
        {
            builder.OwnsMany(x => x.Audits, action =>
            {
                action.WithOwner().HasForeignKey(x => x.RowId);
                action.ToTable($"{builder.GetTableName()}_{nameof(Audit).Pluralize()}");
                action.HasKey(x => x.Id);
                action.Property(x => x.Id).ValueGeneratedOnAdd();
                action.Property(x => x.Additional).HasJsonConversion();
                action.Property(x => x.Changes).HasJsonConversion();
            });
            return builder;
        }

        internal static EntityTypeBuilder<TEntity> ApplyConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IEntityBase
        {
            builder.ApplyIdentifierConfiguration();
            builder.ApplyAuditConfiguration();
            builder.Property(p => p.RowVersion).IsRowVersion();
            builder.HasQueryFilter(entity => !entity.IsDeleted);
            return builder;
        }
    }
}