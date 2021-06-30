using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using R8.EntityFrameworkCore.Audits;

namespace R8.EntityFrameworkCore.EntityBases
{
    internal static class EntityBaseExtensions
    {
        internal static EntityTypeBuilder<TEntity> ApplyIdentifierConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IEntityBaseIdentifier
        {
            builder.ToTable(Extensions.NormalizeTableName(typeof(TEntity)));
            builder.HasKey(x => x.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
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