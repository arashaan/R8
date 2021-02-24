using Humanizer;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace R8.EntityFrameworkCore
{
    internal static class EntityBaseExtensions
    {
        internal static EntityTypeBuilder<TEntity> ApplyConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IEntityBase
        {
            builder.ToTable(typeof(TEntity).Name.Pluralize());
            builder.HasKey(x => x.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.RowVersion).IsRowVersion();
            builder.HasQueryFilter(entity => !entity.IsDeleted);

            var tableName = builder.GetTableName();
            var pluralizeAudit = nameof(Audit).Pluralize();
            builder.OwnsMany(x => x.Audits, action =>
            {
                action.WithOwner().HasForeignKey(x => x.RowId);
                action.ToTable($"{tableName}_{pluralizeAudit}");
                action.HasKey(x => x.Id);
                action.Property(x => x.Id).ValueGeneratedOnAdd();
                action.Property(x => x.Culture).HasCultureConversion();
                action.Property(x => x.NewValues).HasJsonConversion();
                action.Property(x => x.OldValues).HasJsonConversion();
            });
            return builder;
        }
    }
}